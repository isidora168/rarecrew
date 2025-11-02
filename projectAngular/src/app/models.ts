export interface TimeEntry {
  Id: string;
  EmployeeName: string;
  StarTimeUtc: string;
  EndTimeUtc: string;
  EntryNotes: string | null;
  DeletedOn: string | null;
}

export interface EmployeeTotal {
  employee: string;
  totalHours: number; 
}
