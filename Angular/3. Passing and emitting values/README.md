# Passing and emitting values
* Passing values
* Emit values
#### Passing values
in `first.component.html`
```html
<div style="border: solid black 1px; padding:6em;">
    <p>
        first works: {{ firstVar }}
    </p>
    <button (click)="simpleFunc()">
        click me
    </button>
    <!-- Passing value to child -->
    <app-second [firstVarPassed]="firstVar">
    
    </app-second>
</div>
```
in `first.component.html`
