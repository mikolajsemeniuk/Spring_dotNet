# Rest
* Info
* Add module
* Create models
* Create service
* Create component
* Better version

### Info
**API has to return exact same object and arrow function in `addTodo`, `setTodo` and `removeTodo` could return `TodoPayload object` but doesn't has to, to allow changing view without refreshing, all data types to return and sent were double checked.**
### Add module
in `src/app/app.module.ts`
```ts
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

// ADD THIS
import { HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { SharedModule } from './modules/shared.module';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    // ADD THIS
    HttpClientModule,

    SharedModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
```
### Create models
```sh
ng generate interface models/todo-input --type=model
```
in `models/todo-input.model.ts`
```ts
export interface TodoInput {
    title: string
    description: string
    isDone: Boolean
}
```
```sh
ng generate interface models/todo-payload --type=model
```
in `models/todo-input.model.ts`
```ts
export interface TodoPayload {
    id: number
    title: string
    description: string
    created: Date
    updated: Date
    isDone: Boolean
}
```
### Create service
```sh
ng g s services/todo --skipTests=true
```
in `src/app/services/todo.service.ts`
```ts
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { TodoInput } from '../models/todo-input.model';
import { switchMap, tap } from 'rxjs/operators'
import { TodoPayload } from '../models/todo-payload.model';

@Injectable({
  providedIn: 'root'
})
export class TodoService {
  private readonly todo: string = 'todo'
  private values$: BehaviorSubject<TodoPayload[]> = new BehaviorSubject<TodoPayload[]>([]);

  constructor(private http: HttpClient) { }

  getTodos(cache: boolean): Observable<TodoPayload[]> {
    if (this.values$.value && cache) {
      return this.values$.asObservable();
    }
    // { headers: new HttpHeaders().append('Content-Type', 'application/json'), withCredentials: true }
    return this.http.get<TodoPayload[]>(`${environment.apiUrl}/${this.todo}`)
      .pipe(
        switchMap((todos: TodoPayload[]): BehaviorSubject<TodoPayload[]> => {
          this.values$.next(todos)
          return this.values$
        })
      )
  }

  getTodo(id: number): Observable<TodoPayload> {
    return this.http.get<TodoPayload>(`${environment.apiUrl}/${this.todo}/${id}`).pipe(first())
  }

  addTodo(payload: TodoInput): Observable<TodoPayload> {
    return this.http.post<TodoPayload>(`${environment.apiUrl}/${this.todo}`, payload)
      .pipe(
        tap((todo: TodoPayload): void => {
          const values: TodoPayload[] = [...this.values$.value, todo]
          this.values$.next(values)
          // return todo // Could be here but doesn't has to
        })
      )
  }

  setTodo(id: number, payload: TodoInput): Observable<TodoPayload> {
    return this.http.put<TodoPayload>(`${environment.apiUrl}/${this.todo}/${id}`, payload)
      .pipe(
        tap((todo: TodoPayload): void => {
          const values: TodoPayload[] = [...this.values$.value]
          const valueIndex: number = values.findIndex((item: TodoPayload) => item.id === item.id)
          values[valueIndex] = todo
          this.values$.next(values)
          // return todo // Could be here but doesn't has to
        })
      )
  }

  // this method could be also:
  // 
  //     removeTodo(id: number): Observable<null> {
  //       return this.http.delete<null>(`${environment.apiUrl}/${this.todo}/${id}`)
  //         .pipe(
  //           tap(_ => {
  //             const values: TodoPayload[] = this.values$.value.filter(value => value.id !== id)
  //             this.values$.next(values)
  //           })
  //         )
  //     }
  //
  // However api returns deleted object
  // that's why we type `TodoPayload` instead of `null`
  removeTodo(id: number): Observable<TodoPayload> {
    return this.http.delete<TodoPayload>(`${environment.apiUrl}/${this.todo}/${id}`)
      .pipe(
        tap((todo: TodoPayload): void => {
          const values: TodoPayload[] = this.values$.value.filter(value => value.id !== id)
          this.values$.next(values)
          // return todo // Could be here but doesn't has to
        })
      )
  }
}
```
### Create component
```sh
ng g c components/todo --skipTests=true --module ./modules/shared
```
in `src/app/services/todo.component.ts`
```ts
import { Component, OnDestroy, OnInit } from '@angular/core';
import { Observable, Subscription } from 'rxjs';
import { TodoInput } from 'src/app/models/todo-input.model';
import { TodoPayload } from 'src/app/models/todo-payload.model';
import { TodoService } from 'src/app/services/todo.service';

@Component({
  selector: 'app-todo',
  templateUrl: './todo.component.html',
  styleUrls: ['./todo.component.scss']
})
export class TodoComponent implements OnInit, OnDestroy {

  private subscription: Subscription = new Subscription();
  todos$: Observable<TodoPayload[]> = new Observable();

  constructor(private service: TodoService) { }

  ngOnInit(): void {
    this.getTodos(false)
  }

  getTodos(cache: boolean): void {
    this.todos$ = this.service.getTodos()
  }

  addTodo(todo: TodoInput = { title: "linux", description: "lorem ipsum", isDone: false }): void {
    this.subscription.add(this.service.addTodo(todo)
      .subscribe(_ => this.getTodos(true)))
  }

  setTodo(id: number = 5, todo: TodoInput = { title: "linux 2", description: "lorem ipsum 2", isDone: true }): void {
    this.subscription.add(this.service.setTodo(id, todo)
      .subscribe(_ => this.getTodos(true)))
  }

  removeTodo(id: number = 5) {
    this.subscription.add(this.service.removeTodo(id)
      .subscribe(_ => this.getTodos(true)))
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
```
in `src/app/services/todo.component.html`
```html
<p>todo works!</p>

<p (click)="addTodo()">Add todo</p>

<div *ngIf="(todos$ | async) as todos else elseBlock">
    <div>
        <div *ngFor="let todo of todos">
            <h4>
                {{ todo | json }}
            </h4>
            <p style="color:green" (click)="setTodo(todo.id)">
                edit me
            </p>
            <p style="color:red" (click)="removeTodo(todo.id)">
                delete me
            </p>
        </div>
    </div>
    <div *ngIf="todos.length == 0">
        you have no todos :(
    </div>
</div>

<ng-template #elseBlock>
    Loading. Please wait...
</ng-template>
```
### Better version
in `src/app/services/todo.service.ts`
```ts
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { TodoInput } from '../models/todo-input.model';
import { first, switchMap, tap } from 'rxjs/operators'
import { TodoPayload } from '../models/todo-payload.model';

@Injectable({
  providedIn: 'root'
})
export class TodoService {
  private readonly todo: string = 'todo'
  private values$: BehaviorSubject<TodoPayload[]> = new BehaviorSubject<TodoPayload[]>([])

  constructor(private http: HttpClient) {}

  getTodos(cache: boolean): Observable<TodoPayload[]> {
    if (this.values$.value && cache) {
      return this.values$.asObservable();
    }
    // { headers: new HttpHeaders().append('Content-Type', 'application/json'), withCredentials: true }
    return this.http.get<TodoPayload[]>(`${environment.apiUrl}/${this.todo}`)
      .pipe(
        switchMap((todos: TodoPayload[]): BehaviorSubject<TodoPayload[]> => {
          this.values$.next(todos)
          return this.values$
        })
      )
  }

  getTodo = (id: number): Observable<TodoPayload> =>
    this.http.get<TodoPayload>(`${environment.apiUrl}/${this.todo}/${id}`).pipe(first())

  addTodo = (payload: TodoInput): Observable<TodoPayload> =>
    this.http.post<TodoPayload>(`${environment.apiUrl}/${this.todo}`, payload)
      .pipe(
        first(),
        tap((todo: TodoPayload): void => {
          const values: TodoPayload[] = [...this.values$.value, todo]
          this.values$.next(values)
        })
      )

  setTodo = (id: number, payload: TodoInput): Observable<TodoPayload> =>
    this.http.put<TodoPayload>(`${environment.apiUrl}/${this.todo}/${id}`, payload)
      .pipe(
        first(),
        tap((todo: TodoPayload): void => {
          const values: TodoPayload[] = [...this.values$.value]
          const valueIndex: number = values.findIndex((item: TodoPayload) => item.id === item.id)
          values[valueIndex] = todo
          this.values$.next(values)
        })
      )

  removeTodo = (id: number): Observable<TodoPayload> =>
    this.http.delete<TodoPayload>(`${environment.apiUrl}/${this.todo}/${id}`)
      .pipe(
        first(),
        tap((todo: TodoPayload): void => {
          const values: TodoPayload[] = this.values$.value.filter(value => value.id !== id)
          this.values$.next(values)
        })
      )
}
```
in `src/app/services/todo.component.ts`
```ts
import { Component, OnInit } from '@angular/core';
import { Observable, Subscription } from 'rxjs';
import { TodoInput } from 'src/app/models/todo-input.model';
import { TodoPayload } from 'src/app/models/todo-payload.model';
import { TodoService } from 'src/app/services/todo.service';

@Component({
  selector: 'app-todo',
  templateUrl: './todo.component.html',
  styleUrls: ['./todo.component.scss']
})
export class TodoComponent implements OnInit {
  todos$: Observable<TodoPayload[]> = new Observable();

  constructor(private service: TodoService) { }

  ngOnInit(): void {
    this.getTodos(false)
  }

  getTodos(cache: boolean): void {
    this.todos$ = this.service.getTodos(cache)
  }

  addTodo(todo: TodoInput = { title: "linux", description: "lorem ipsum", isDone: false }): void {
    this.service.addTodo(todo)
      .subscribe(
        _ => this.getTodos(true),
        err => {},
        () => {}
      )
  }

  setTodo(id: number = 5, todo: TodoInput = { title: "linux 2", description: "lorem ipsum 2", isDone: true }): void {
    this.service.setTodo(id, todo)
      .subscribe(
        _ => this.getTodos(true),
        err => { },
        () => { }
      )
  }

  removeTodo(id: number = 5) {
    this.service.removeTodo(id)
      .subscribe(
        _ => this.getTodos(true),
        err => { },
        () => { }
      )
  }
}
```
Rest the same
