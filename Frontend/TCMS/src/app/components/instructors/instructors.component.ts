import { Component } from '@angular/core';
// import (instructor interface) from (instructor.ts file)
import { Instructor } from '../../interfaces/instructor';

@Component({
  selector: 'app-instructors',
  imports: [],
  templateUrl: './instructors.component.html',
  styleUrl: './instructors.component.css'
})
export class InstructorsComponent {

  instructors : Instructor[] = [
    {id: 1, fullName: 'John Doe', email: 'john.doe@example.com', phoneNumber: '0785603431', status: true, specialization: 'Mathematics', assignedCoursesCount: 5},
    {id: 2, fullName: 'Jane Smith', email: 'jane.smith@example.com', phoneNumber: '0785685931', status: true, specialization: 'Science', assignedCoursesCount: 3},
    {id: 3, fullName: 'Michael Johnson', email: 'michael.johnson@example.com', phoneNumber: '078885431', status: false, specialization: 'History', assignedCoursesCount: 2},
    {id: 4, fullName: 'Emily Davis', email: 'emily.davis@example.com', phoneNumber: '0782285431', status: true, specialization: 'English', assignedCoursesCount: 4},
    {id: 5, fullName: 'David Wilson', email: 'david.wilson@example.com', phoneNumber: '0782681431', status: false, specialization: 'Physics', assignedCoursesCount: 3}
  ];

  instructorsTableColumns : string[] = [
    '#',
    'Name',
    'Email',
    'Phone Number',
    'Status',
    'Specialization',
    'Assigned Courses'
  ];

  constructor() { }
}

