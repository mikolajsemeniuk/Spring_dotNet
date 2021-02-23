# Passing and emitting values
* Passing values
* Emit values
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
in `first.component.html`
