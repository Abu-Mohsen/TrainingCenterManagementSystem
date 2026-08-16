import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Instructor, InstructorDetail, InstructorPayload } from '../interfaces/instructor';


@Injectable({
  providedIn: 'root'
})

export class InstructorService {

  private baseUrl = 'https://localhost:44364/api/Instructors';
  constructor(private http: HttpClient) { }

  // getInstructors - With Optional Filters
  getAll(filters?: { name?: string; specialization?: string; status?: string }): Observable<Instructor[]> {
    let params = new HttpParams();

    if (filters?.name) params = params.set('name', filters.name);
    if (filters?.specialization) params = params.set('specialization', filters.specialization);
    if (filters?.status) params = params.set('status', filters.status);

    return this.http.get<Instructor[]>(this.baseUrl, { params });
  }

  // GetInstructor - Retrieve details of a specific instructor (Used View-Icon (عين يا مهندس مو قلترة بالبحث بس تم ربطها))
  getById(id: number): Observable<InstructorDetail> {
    return this.http.get<InstructorDetail>(`${this.baseUrl}/${id}`);
  }

  // CreateInstructor - Create a new instructor
  create(payload: InstructorPayload): Observable<Instructor> {
    return this.http.post<Instructor>(this.baseUrl, payload);
  }

  // UpdateInstructor - Update an existing instructor
  update(id: number, payload: InstructorPayload): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, payload);
  }

  // DeleteInstructor - Delete an existing instructor
  delete(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${id}`);
  }
}
