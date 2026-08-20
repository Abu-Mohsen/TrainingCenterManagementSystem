import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Observable } from 'rxjs';
import { Student, StudentDetail, StudentPayload } from '../../interfaces/student';
import { StudentService } from '../../services/student.service';
import { Modal } from 'bootstrap';

@Component({
  selector: 'app-students',
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './students.component.html',
  styleUrl: './students.component.css'
})
export class StudentsComponent implements OnInit {

  students: Student[] = [];
  isLoading = false
  errorMessage = '';

  studentsTableColumns: string[] = [
    '#',
    'Name',
    'Email',
    'Phone Number',
    'Specialty',
    'Enrolled Courses'
  ];

  // Filter State
  filterName = '';
  filterSpecialty = '';

  // Add/Edit Modal State
  studentForm: FormGroup
  editingStudentId: number | null = null; // null => Add mode, otherwise Edit mode
  isSaving = false
  saveError = '';

  // View Details Modal State
  selectedStudentDetail: StudentDetail | null = null;

  // Constructor with FormBuilder and StudentService injection
  constructor(private studentService: StudentService, private fb: FormBuilder) {
    this.studentForm = this.fb.group({
      fullName: ['', [Validators.required, Validators.maxLength(100)]],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(40)]],
      phoneNumber: ['', [Validators.maxLength(10)]],
      specialty: ['', [Validators.maxLength(100)]]
    });
  }

  ngOnInit(): void {
    this.loadStudents();
  }

  loadStudents(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.studentService.getAll({
      name: this.filterName,
      specialty: this.filterSpecialty
  }).subscribe({
    next: (data) => {
      this.students = data;
      this.isLoading = false;
    },
    error: (err: any) => {
      this.errorMessage = 'حدث خطأ اثناء تحميل قائمة الطلاب. الرجاء المحاولة لاحقاً.';
      this.isLoading = false;
      console.error( err);
    }
  });
}

  onSearch(): void {
    this.loadStudents();
  }

  onResetFilters(): void {
    this.filterName = '';
    this.filterSpecialty = '';
    this.loadStudents();
  }

  // ---------- Add ----------
  openAddModal(): void {
    this.editingStudentId = null;
    this.saveError = '';
    this.studentForm.reset();
    this.showModal('saveStudentModal');
  }

  // ---------- Edit ----------
  openEditModal(student: Student): void {
    this.editingStudentId = student.id;
    this.saveError = '';
    this.studentForm.setValue({
      fullName: student.fullName,
      email: student.email,
      phoneNumber: student.phoneNumber,
      specialty: student.specialty
    });
    this.showModal('saveStudentModal');
  }

  save(): void {
    if (this.studentForm.invalid) {
      this.studentForm.markAllAsTouched();
      return;
    }

    this.isSaving = true;
    this.saveError = '';
    const payload: StudentPayload = this.studentForm.value;

    const request$: Observable<any> = this.editingStudentId
      ? this.studentService.update(this.editingStudentId, payload)
      : this.studentService.create(payload);

    request$.subscribe({
      next: () => {
        this.isSaving = false;
        this.hideModal('saveStudentModal');
        this.loadStudents();
      },
      error: (err: any) => {
        this.isSaving = false;
        this.saveError = err?.error?.message || 'حدث خطأ أثناء الحفظ.';
      }
    });
  }

  // ---------- View details (eye icon -> GetStudent(id)) ----------
  viewStudent(id: number): void {
    this.selectedStudentDetail = null;

    this.studentService.getById(id).subscribe({
      next: (data) => {
        this.selectedStudentDetail = data;
        this.showModal('viewStudentModal');
      },
      error: (err: any) => {
        console.error(err);
        alert('تعذّر تحميل تفاصيل الطالب.');
      }
    });
  }

  // ---------- Delete ----------
  deleteStudent(student: Student): void {
    const confirmed = confirm(`متأكد إنك بدك تحذف الطالب "${student.fullName}"؟`);
    if (!confirmed) return;

    this.studentService.delete(student.id).subscribe({
      next: () => this.loadStudents(),
      error: (err: any) => {
        alert(err?.error?.message || 'تعذّر حذف الطالب.');
      }
    });
  }

  // ---------- Bootstrap modal helpers ----------
  private showModal(elementId: string): void {
    const modalEl = document.getElementById(elementId);
    if (modalEl) Modal.getOrCreateInstance(modalEl).show();
  }

  private hideModal(elementId: string): void {
    const modalEl = document.getElementById(elementId);
    if (modalEl) Modal.getOrCreateInstance(modalEl).hide();
  }
}
