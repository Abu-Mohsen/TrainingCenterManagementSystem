import { Routes } from '@angular/router';
import { InstructorsComponent } from './components/instructors/instructors.component';
import { StudentsComponent } from './components/students/students.component';
import { NotFoundComponent } from './components/not-found/not-found.component';

export const routes: Routes = [

  { path: '', redirectTo: 'instructors', pathMatch: 'full' },
  { path: 'instructors', component: InstructorsComponent },
  { path: 'students', component: StudentsComponent },





  { path: '**', component: NotFoundComponent }
];
