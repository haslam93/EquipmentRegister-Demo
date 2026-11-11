import { DecimalPipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { EquipmentApi } from '../equipment-api';
import { EquipmentItem } from '../models';

@Component({
  imports: [DecimalPipe],
  selector: 'app-register-page',
  templateUrl: './register-page.html',
})
export class RegisterPage {
  private readonly api = inject(EquipmentApi);

  protected readonly items = signal<EquipmentItem[]>([]);
  protected readonly search = signal('');
  protected readonly loading = signal(true);
  protected readonly error = signal('');

  protected readonly visibleItems = computed(() => {
    const term = this.search().trim().toLowerCase();
    if (!term) {
      return this.items();
    }

    return this.items().filter(
      (item) =>
        item.tag.toLowerCase().includes(term) || item.description.toLowerCase().includes(term),
    );
  });

  protected readonly totalPowerKw = computed(() =>
    this.visibleItems().reduce((total, item) => total + (item.powerKw ?? 0), 0),
  );

  constructor() {
    this.api.list().subscribe({
      next: (items) => {
        this.items.set(items);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('The register could not be loaded. Is the API running on port 5080?');
        this.loading.set(false);
      },
    });
  }

  protected onSearch(event: Event): void {
    this.search.set((event.target as HTMLInputElement).value);
  }
}
