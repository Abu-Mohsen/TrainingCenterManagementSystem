// Interface for the Instructor entity / Same As [InstructorReadDto]
export interface Instructor {
  id: number;
  fullName: string;
  email: string;
  phoneNumber: string;
  specialization: string;
  assignedCoursesCount: number;
  status: string; // "Active" | "Inactive" - Computed in backend (not stored heheee)
}

// Interface for a single course assigned to an instructor
export interface InstructorCourse {
  courseId: number;
  title: string;
  maxCapacity: number;
  fee: number;
  startDate: string;
  endDate: string;
  enrolledStudentsCount: number;
}

// Interface for the complete response of GetInstructor(id) // View
export interface InstructorDetail {
  instructorDetails: Instructor;
  assignedCourses: InstructorCourse[];
}

// InstructorCreateDto / InstructorUpdateDto
export interface InstructorPayload {
  fullName: string;
  email: string;
  phoneNumber: string;
  specialization: string;
}