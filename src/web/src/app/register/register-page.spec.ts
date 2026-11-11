import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EquipmentItem } from '../models';
import { RegisterPage } from './register-page';

const items: EquipmentItem[] = [
  { tag: 'ML-2001', description: 'SAG mill', discipline: 'Mechanical', area: '200', powerKw: 12000, weightKg: 890000, status: 'Delivered' },
  { tag: 'PU-2010A', description: 'Cyclone feed pump (duty)', discipline: 'Process', area: '200', powerKw: 900, weightKg: 14500, status: 'Ordered' },
  { tag: 'TX-5001', description: 'Main step-down transformer', discipline: 'Electrical', area: '500', powerKw: null, weightKg: 74000, status: 'Ordered' },
];

describe('RegisterPage', () => {
  let fixture: ComponentFixture<RegisterPage>;
  let page: HTMLElement;

  beforeEach(async () => {
    TestBed.configureTestingModule({
      imports: [RegisterPage],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });

    fixture = TestBed.createComponent(RegisterPage);
    page = fixture.nativeElement as HTMLElement;
    TestBed.inject(HttpTestingController).expectOne('/api/equipment').flush(items);
    await fixture.whenStable();
  });

  it('lists every item returned by the API', () => {
    expect(page.querySelectorAll('tbody tr').length).toBe(3);
    expect(page.querySelector('[data-testid="summary"]')?.textContent).toContain('3 of 3 items');
  });

  it('filters by tag or description as the user types', async () => {
    const search = page.querySelector('input[type="search"]') as HTMLInputElement;
    search.value = 'pump';
    search.dispatchEvent(new Event('input'));
    await fixture.whenStable();

    const rows = page.querySelectorAll('tbody tr');
    expect(rows.length).toBe(1);
    expect(rows[0].textContent).toContain('PU-2010A');
  });
});
