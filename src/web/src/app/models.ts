export interface EquipmentItem {
  tag: string;
  description: string;
  discipline: string;
  area: string;
  powerKw: number | null;
  weightKg: number | null;
  status: string;
}

export interface RowError {
  rowNumber: number;
  ruleId: string;
  column: string;
  message: string;
}

export interface ImportResult {
  fileName: string;
  totalRows: number;
  accepted: number;
  rejected: number;
  errors: RowError[];
}
