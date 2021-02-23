# Passing and emitting values
* Passing values
* Emitting values
#### Passing values
in `first.component.ts`
```ts
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-first',
  templateUrl: './first.component.html',
  styleUrls: ['./first.component.scss']
})
export class FirstComponent implements OnInit {

  firstVar: number = 14;

  constructor() { }

  ngOnInit(): void {
  }
}

```
in `first.component.html`
```html
<div>
    <!-- Passing value to child -->
    <app-second [firstVarPassed]="firstVar">
    
    </app-second>
</div>
```
in `second.component.ts`
```ts
import { Input, Output, EventEmitter } from '@angular/core';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-second',
  templateUrl: './second.component.html',
  styleUrls: ['./second.component.scss']
})
export class SecondComponent implements OnInit {

  @Input()
  firstVarPassed: number;

  constructor() { }

  ngOnInit(): void {
  }
}
```
in `second.component.html`
```html
<p>second works: {{ firstVarPassed }}</p>
```
#### Emitting values
in `first.component.ts`
```ts
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-first',
  templateUrl: './first.component.html',
  styleUrls: ['./first.component.scss']
})
export class FirstComponent implements OnInit {

  firstVar: number = 14;

  constructor() { }

  ngOnInit(): void {
  }

  firstFunc(event: number): void {
    this.firstVar = event
  }
}
```
in `first.component.html`
```html
<div style="border: solid black 1px; padding:6em;">
    <p>
        first works: {{ firstVar }}
    </p>
    <!-- Passing value and function for emitting -->
    <app-second (firstFuncPassed)="firstFunc($event)" [firstVarPassed]="firstVar">
    
    </app-second>
</div>
```
in `second.component.ts`
```ts
import { Input, Output, EventEmitter } from '@angular/core';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-second',
  templateUrl: './second.component.html',
  styleUrls: ['./second.component.scss']
})
export class SecondComponent implements OnInit {

  @Input()
  firstVarPassed: number;

  constructor() { }

  ngOnInit(): void {
  }

  @Output() firstFuncPassed = new EventEmitter<number>()

  secondFunc(event: number) {
    // if arg `event` is not null use `event` else use `2`
    this.firstFuncPassed.emit(event || 2)
  }
}
```
