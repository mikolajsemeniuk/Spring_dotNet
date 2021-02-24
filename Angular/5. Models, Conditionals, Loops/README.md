# Models, Conditionals, Loops
* Models
* Conditionals
* Loops

### Models
in `app.module.ts`
```ts
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { FormsModule } from '@angular/forms'; // ADD THIS

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { WorkshopComponent } from './components/workshop/workshop.component';

@NgModule({
  declarations: [
    AppComponent,
    WorkshopComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,

    FormsModule // ADD THIS
    
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
```
in `workshop.component.ts`
```ts
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-workshop',
  templateUrl: './workshop.component.html',
  styleUrls: ['./workshop.component.scss']
})
export class WorkshopComponent implements OnInit {

  // model property
  current: number = 1

  constructor() { }

  ngOnInit(): void {
  }
}
```
in `workshop.component.html`
```html
<p>workshop works: {{ current }}</p>

<input [(ngModel)]="current" name="current" type="number">
```
