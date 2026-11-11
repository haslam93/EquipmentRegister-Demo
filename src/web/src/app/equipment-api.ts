import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { EquipmentItem, ImportResult } from './models';

@Injectable({ providedIn: 'root' })
export class EquipmentApi {
  private readonly http = inject(HttpClient);

  list(): Observable<EquipmentItem[]> {
    return this.http.get<EquipmentItem[]>('/api/equipment');
  }

  import(file: File): Observable<ImportResult> {
    const form = new FormData();
    form.append('file', file, file.name);
    return this.http.post<ImportResult>('/api/imports', form);
  }
}
