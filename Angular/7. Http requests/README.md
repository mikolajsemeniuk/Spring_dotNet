# Http requests
* Import module
* Create model
* Create service
* Send requests

### Import module
in `app.module.ts`
```ts
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { HttpClientModule } from '@angular/common/http'; // ADD THIS

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './components/home/home.component';
import { FirstComponent } from './components/first/first.component';
import { SecondComponent } from './components/second/second.component';
import { ThirdComponent } from './components/third/third.component';
import { WorkshopComponent } from './components/workshop/workshop.component';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    FirstComponent,
    SecondComponent,
    ThirdComponent,
    WorkshopComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,

    HttpClientModule // ADD THIS
    
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
```
### Create model
Create model
```sh
# with tests
ng generate class models/Todo --type=model
# without tests
ng generate class models/Todo --type=model --skipTests=true
```
`Post` body in `models/post.model.ts`
```ts
export class Post {
    id: number
    title: string
    body: string
    userId: number
}
```
### Create service
```sh
ng g s services/Todo --skipTests=true
```
### Send requests
in `todo.model.ts`
```ts
export class Todo {
    userId: number
    id: number
    title: string
    completed: boolean
}
```
in `todo.service.ts`
```ts
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Todo } from '../models/todo.model';

@Injectable({
  providedIn: 'root'
})
export class TodoService {

  private readonly url: string = 'https://jsonplaceholder.typicode.com/todos'
  private limit: string = '?_start=0&_limit=5'

  constructor(private http: HttpClient) { }

  getTodos(): Observable<Todo[]> {
    let headers = new HttpHeaders().set('Content-Type', 'application/json')
    return this.http.get<Todo[]>(`${this.url}${this.limit}`, { headers })
  }

  putTodo(todo: Todo): Observable<any>{
    let headers = new HttpHeaders().set('Content-Type', 'application/json')
    return this.http.put<Todo>(`${this.url}/${todo.id}`, todo, { headers })
  }

  postTodo(todo: Todo): Observable<Todo> {
    let headers = new HttpHeaders().set('Content-Type', 'application/json')
    return this.http.post<Todo>(`${this.url}`, todo, { headers })
  }

  deleteTodo(todo: Todo) {
    let headers = new HttpHeaders().set('Content-Type', 'application/json')
    return this.http.delete<Todo>(`${this.url}/${todo.id}`, { headers })
  }

}
```
in `todo.component.ts`
```ts
import { Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { Todo } from 'src/app/models/todo.model';
import { TodoService } from 'src/app/services/todo.service';

@Component({
  selector: 'app-todo',
  templateUrl: './todo.component.html',
  styleUrls: ['./todo.component.scss']
})
export class TodoComponent implements OnInit {
  
  todos: Todo[];
  subscribe: Subscription

  todo: Todo = {
    userId: 1,
    id: 174,
    title: 'title',
    completed: true
  };

  constructor(private todoService: TodoService) { }

  ngOnInit(): void {
    this.getTodos();
  }

  getTodos() {
    this.subscribe = this.todoService.getTodos().subscribe(
      todos => this.todos = todos
    )
  }

  updateCompleted(todo: Todo) {
    // in UI
    todo.completed = !todo.completed
    // in server
    this.todoService.putTodo(todo).subscribe(data => console.log(data))
  }

  updateTitle(title: string, todo: Todo) {
    // in UI
    todo.title = title
    // in server
    this.todoService.putTodo(todo).subscribe(data => console.log(data))
  }

  addTodo(): void {
    this.todoService.putTodo(this.todo).subscribe(data => this.todos.push(data))
  }

  deleteTodo(todo: Todo): void {
    this.todoService.deleteTodo(todo).subscribe(data => {
      const index = this.todos.findIndex(el => el.id == todo.id)
      this.todos.splice(index, 1)
    })
  }

  ngOnDestroy() {
    this.subscribe.unsubscribe()
  }

}
```
in `todo.component.html`
```html
<p>todo works!</p>

<div *ngFor="let todo of todos">
    <p>
        {{ todo | json }}
    </p>
    <input (input)="updateTitle($event.target.value, todo)" [value]="todo.title" />
    <input (click)="updateCompleted(todo)" type="checkbox" [value]="todo.completed" />
    <div (click)="deleteTodo(todo)">x</div>
</div>

<div style="margin: 50px 0px;">
    <input [(ngModel)]="todo.userId" name="userId">
    <input [(ngModel)]="todo.id" name="id">
    <input [(ngModel)]="todo.title" name="title">
    <input [(ngModel)]="todo.completed" type="checkbox" name="completed">
    <button (click)="addTodo()">add me</button>
</div>
```
