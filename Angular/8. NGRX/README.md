# NGRX
* Install store and devtools
* Simple store

### Install store and devtools
```sh
ng add @ngrx/store@latest --minimal false
ng add @ngrx/store-devtools@latest
ng add @ngrx/schematics@latest # optional if you want to generate store
```
### Simple store
in `src/app/reducers/index.ts`
```ts
import {
  ActionReducer,
  ActionReducerMap,
  createAction,
  createFeatureSelector,
  createReducer,
  createSelector,
  MetaReducer,
  on,
  props
} from '@ngrx/store';

import { environment } from '../../environments/environment';


// Local State declaration and implementation
export interface numbersState {
  values: number[],
  isVisible: boolean
}
const initialState: numbersState = {
  values: [1, 2, 3],
  isVisible: true
}


// Actions and Reducer
export const changeStatus = createAction(
  "[State] Change Status"
)
export const addNumber = createAction(
  "[State] Add Number",
  props<{ index: number, value: number }>()
)
export const removeNumber = createAction(
  "[State] Remove Number",
  props<{ index: number }>()
)
export const editNumber = createAction(
  "[State] Edit Number",
  props<{ index: number, value: number }>()
)
export const numbersReducer = createReducer(
  initialState,
  on(changeStatus, (state: numbersState): numbersState => {
    return {
      ...state,
      isVisible: !state.isVisible
    }
  }),
  on(addNumber, (state: numbersState, action): numbersState => {
    return {
      ...state,
      values: [...state.values].flatMap((v, i) => i === action.index ? [action.value, v] : v)
    }
  }),
  on(removeNumber, (state: numbersState, action): numbersState => {
    return {
      ...state,
      values: [...state.values].filter((_, i) => i != action.index)
    }
  }),
  on(editNumber, (state: numbersState, action): numbersState => {
    return {
      ...state,
      values: [...state.values].map((v, i) => i === action.index ? v = action.value : v)
    }
  })
)

// Global State declaration and ActionReducerMap implementation
export interface State {
  numbers: numbersState
}
export const reducers: ActionReducerMap<State> = {
  numbers: numbersReducer
};

// Getters
const getNumbersState = createFeatureSelector<numbersState>('numbers') // feature getter
export const getValues = createSelector( // properties selector
  getNumbersState,
  state => state.values
)
export const getIsVisible = createSelector( // properties selector
  getNumbersState,
  state => state.isVisible
)


// Additional
export const metaReducers: MetaReducer<State>[] = !environment.production ? [] : [];
```
in `src/app/app.component.ts`
```ts
import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { getValues, changeStatus, addNumber, removeNumber, editNumber, numbersState, getIsVisible } from './reducers';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  items$: Observable<number[]> = new Observable()
  isVisible$: Observable<boolean> = new Observable()
  index: number = 0
  value: number = 7

  constructor(private store: Store<numbersState>) { }
  
  ngOnInit(): void {
    this.items$ = this.store.select(getValues)
    this.isVisible$ = this.store.select(getIsVisible)
  }

  show(): void {
    this.store.dispatch(changeStatus())
  }

  add(): void {
    this.store.dispatch(addNumber({ index: this.index, value: this.value }));
  }

  remove(): void {
    this.store.dispatch(removeNumber({ index: this.index }))
  }

  edit(): void {
    this.store.dispatch(editNumber({ index: this.index, value: this.value }))
  }
}
```
in `src/app/app.component.html`
```html
<h2>Simple Store</h2>
<input [(ngModel)]="index" type="number">
<input [(ngModel)]="value" type="number">
<br>
<br>
<button (click)="add()">add me</button>
<button (click)="remove()">remove me</button>
<button (click)="edit()">edit me</button>
<br>
<br>
<button (click)="show()">show numbers: {{ isVisible$ | async }}</button>
<div *ngIf="(isVisible$ | async) == true">
  <p *ngFor="let item of items$ | async">
    {{ item }}
  </p>
</div>

<router-outlet></router-outlet>
```
Additional: `tsconfig.json`
```json
/* To learn more about this file see: https://angular.io/config/tsconfig. */
{
  "compileOnSave": false,
  "compilerOptions": {
    "baseUrl": "./",
    "outDir": "./dist/out-tsc",
    "forceConsistentCasingInFileNames": true,
    "strict": true,
    "noImplicitReturns": true,
    "noFallthroughCasesInSwitch": true,
    "sourceMap": true,
    "declaration": false,
    "downlevelIteration": true,
    "experimentalDecorators": true,
    "moduleResolution": "node",
    "importHelpers": true,
    "target": "es2015",
    "module": "es2020",
    "lib": [
      "es2019",
      "dom"
    ]
  },
  "angularCompilerOptions": {
    "enableI18nLegacyMessageIdFormat": false,
    "strictInjectionParameters": true,
    "strictInputAccessModifiers": true,
    "strictTemplates": true
  }
}
```
