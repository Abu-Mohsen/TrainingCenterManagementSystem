// Equivalent to the [StudentReadDto] in the backend
export interface Student {

    id: number;
    fullName: string;
    email: string;
    phoneNumber: string;
    specialty: string;
    enrolledCoursesCount: number;
}

 // 1 Course in Student's Details (Part of Response(GetStudent(id)))
export interface EnrolledCourse {
    enrollmentId: number;
    courseId: number;
    courseTitle: string;
    courseFee: number;
    paidAmount: number;
    remainingAmount: number;
    isFullyPaid: boolean;
    enrollmentDate: string;
    status: string; // "Active" | "Completed"
}

// Financial summary for a student
export interface FinancialSummary {
    totalRequiredFees: number;
    totalPaidAmount: number;
    totalRemainingBalance: number;
}

// Complete Shape of the Response(GetStudent(id)) from the backend
export interface StudentDetail {
    studentDetails: Student;
    financialSummary: FinancialSummary;
    enrolledCourses: EnrolledCourse[];
}

// The Shape of the Request Body for Adding/Editing a Student (Post/Put)
export interface StudentPayload {
    fullName: string;
    email: string;
    phoneNumber: string;
    specialty: string;
}