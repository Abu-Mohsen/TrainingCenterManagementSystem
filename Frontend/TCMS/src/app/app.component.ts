import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

// import Components
import { InstructorsComponent } from './components/instructors/instructors.component';


// [Decorator] that marks a class as an [Angular component] and provides configuration metadata that determines how the component should be processed, instantiated, and used at runtime.
@Component({
  selector: 'app-root',
  imports: [RouterOutlet,
            InstructorsComponent],

  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {

  
}
