import { Component, inject, signal } from '@angular/core';
import { EquipmentApi } from '../equipment-api';
import { ImportResult } from '../models';

@Component({
  selector: 'app-import-page',
  templateUrl: './import-page.html',
  styleUrl: './import-page.css',
})
export class ImportPage {
  private readonly api = inject(EquipmentApi);

  protected readonly file = signal<File | null>(null);
  protected readonly busy = signal(false);
  protected readonly result = signal<ImportResult | null>(null);
  protected readonly error = signal('');

  protected onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.file.set(input.files?.[0] ?? null);
    this.result.set(null);
    this.error.set('');
  }

  protected upload(): void {
    const file = this.file();
    if (!file) {
      return;
    }

    this.busy.set(true);
    this.error.set('');
    this.api.import(file).subscribe({
      next: (result) => {
        this.result.set(result);
        this.busy.set(false);
      },
      error: (response) => {
        this.error.set(response?.error?.title ?? 'The import failed. Is the API running on port 5080?');
        this.busy.set(false);
      },
    });
  }
}
