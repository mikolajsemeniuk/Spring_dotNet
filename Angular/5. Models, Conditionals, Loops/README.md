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
### Conditionals
in `workshop.component.ts`
```ts
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-workshop',
  templateUrl: './workshop.component.html',
  styleUrls: ['./workshop.component.scss']
})
export class WorkshopComponent implements OnInit {

  current: number = 1
  foo: number = 7

  constructor() { }

  ngOnInit(): void {
  }

}
```
in `workshop.component.html`
```html
<p>workshop works: {{ current }}</p>

<input [(ngModel)]="current" name="current" type="number">
<input [(ngModel)]="foo" name="foo" type="number">

<!-- if -->
<h1 *ngIf="current == 1">current == 1</h1>

<!-- if else -->
<h2 *ngIf="current == 1; else other">current == 1</h2>
<ng-template #other>
    <h2>current == else</h2>
</ng-template>

<!-- if, else if, else -->
<ng-container *ngIf="foo === 1; else elseif1">
    foo === 1
</ng-container>

<ng-template #elseif1>
    <ng-container *ngIf="foo === 99; else elseif2">
        foo === 99
    </ng-container>
</ng-template>

<ng-template #elseif2>
    <ng-container *ngIf="foo === 2; else else1">
        foo === 2
    </ng-container>
</ng-template>
<ng-template #else1>
    else
</ng-template>
```
### Loops
