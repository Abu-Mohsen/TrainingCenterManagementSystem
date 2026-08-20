import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Student, StudentDetail, StudentPayload } from '../interfaces/student';

@Injectable({
  providedIn: 'root'
})
export class StudentService {

  private baseUrl = 'https://localhost:44364/api/students';

  constructor(private http: HttpClient) { }

  // Get All Students (with optional filters)- ByCretiria
  getAll(filters?: { name?: string; specialty?: string }): Observable<Student[]> {
    let params = new HttpParams();

    if (filters?.name) params = params.set('name', filters.name);
    if (filters?.specialty) params = params.set('specialty', filters.specialty);

    return this.http.get<Student[]>(`${this.baseUrl}/ByCriteria`, { params });
  }

  // Returns the details of a specific student by ID, including their financial summary and enrolled courses. - View
  getById(id: number): Observable<StudentDetail> {
    return this.http.get<StudentDetail>(`${this.baseUrl}/${id}`);
  }

  // Add a new student to the system. - Add/create
  create(payload: StudentPayload): Observable<Student> {
    return this.http.post<Student>(this.baseUrl, payload);
  }

  // Update the details of an existing student by ID. - Edit
  update(id: number, payload: StudentPayload): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, payload);
  }

  // Delete a student from the system by ID. - Delete
  delete(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${id}`);
  }

}
