import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Instructor, InstructorDetail, InstructorPayload } from '../../interfaces/instructor';
import { InstructorService } from '../../services/instructor.service';
import { Observable } from 'rxjs';
import { Modal } from 'bootstrap'; // Import Bootstrap's Modal class

// Bootstrap's JS bundle is assumed to be loaded globally (angular.json "scripts", or a <script> tag
// in index.html) - same way [data-bs-toggle] already works in the existing template.


@Component({
  selector: 'app-instructors',
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './instructors.component.html',
  styleUrl: './instructors.component.css'
})
export class InstructorsComponent implements OnInit {

  instructors: Instructor[] = [];
  isLoading = false;
  errorMessage = '';

  instructorsTableColumns: string[] = [
    '#',
    'Name',
    'Email',
    'Phone Number',
    'Status',
    'Specialization',
    'Assigned Courses'
  ];

  // Filter State
  filterName = '';
  filterSpecialization = '';
  filterStatus = '';

  // Add/Edit Modal State
  instructorForm: FormGroup;
  editingInstructorId: number | null = null; // null => Add mode, otherwise Edit mode
  isSaving = false;
  saveError = '';

  // View Details Modal State
  selectedInstructorDetail: InstructorDetail | null = null;

  constructor(private instructorService: InstructorService, private fb: FormBuilder) {
    this.instructorForm = this.fb.group({
      fullName: ['', [Validators.required, Validators.maxLength(100)]],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(40)]],
      phoneNumber: ['', [Validators.maxLength(10)]],
      specialization: ['', [Validators.maxLength(100)]]
    });
  }

  ngOnInit(): void {
    this.loadInstructors();
  }

  loadInstructors(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.instructorService.getAll({
      name: this.filterName,
      specialization: this.filterSpecialization,
      status: this.filterStatus
    }).subscribe({
      next: (data) => {
        this.instructors = data;
        this.isLoading = false;
      },
      error: (err: any) => {
        this.errorMessage = 'حدث خطأ أثناء تحميل قائمة المدرّبين.';
        this.isLoading = false;
        console.error(err);
      }
    });
  }

  onSearch(): void {
    this.loadInstructors();
  }

  onResetFilters(): void {
    this.filterName = '';
    this.filterSpecialization = '';
    this.filterStatus = '';
    this.loadInstructors();
  }

  // ---------- Add ----------
  openAddModal(): void {
    this.editingInstructorId = null;
    this.saveError = '';
    this.instructorForm.reset();
    this.showModal('saveInstructorModal');
  }

  // ---------- Edit ----------
  openEditModal(instructor: Instructor): void {
    this.editingInstructorId = instructor.id;
    this.saveError = '';
    this.instructorForm.setValue({
      fullName: instructor.fullName,
      email: instructor.email,
      phoneNumber: instructor.phoneNumber,
      specialization: instructor.specialization
    });
    this.showModal('saveInstructorModal');
  }

  save(): void {
    if (this.instructorForm.invalid) {
      this.instructorForm.markAllAsTouched();
      return;
    }

    this.isSaving = true;
    this.saveError = '';
    const payload: InstructorPayload = this.instructorForm.value;

    const request$: Observable<any> = this.editingInstructorId
  ? this.instructorService.update(this.editingInstructorId, payload)
  : this.instructorService.create(payload);

    request$.subscribe({
      next: () => {
        this.isSaving = false;
        this.hideModal('saveInstructorModal');
        this.loadInstructors();
      },
      error: (err: any) => {
        this.isSaving = false;
        this.saveError = err?.error?.message || 'حدث خطأ أثناء الحفظ.';
      }
    });
  }

  // ---------- View details (eye icon -> GetInstructor(id)) ----------
  viewInstructor(id: number): void {
    this.selectedInstructorDetail = null;

    this.instructorService.getById(id).subscribe({
      next: (data) => {
        this.selectedInstructorDetail = data;
        this.showModal('viewInstructorModal');
      },
      error: (err: any) => {
        console.error(err);
        alert('تعذّر تحميل تفاصيل المدرّب.');
      }
    });
  }

  // ---------- Delete ----------
  deleteInstructor(instructor: Instructor): void {
    const confirmed = confirm(`متأكد إنك بدك تحذف المدرّب "${instructor.fullName}"؟`);
    if (!confirmed) return;

    this.instructorService.delete(instructor.id).subscribe({
      next: () => this.loadInstructors(),
      error: (err: any) => {
        alert(err?.error?.message || 'تعذّر حذف المدرّب.');
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