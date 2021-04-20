# GraphQL
* Add module
* Create service
* Create component


### Add module
in `shell`
```sh
ng add apollo-angular # https://localhost:5001/graphql/
```
### Create service
in `shell`
```sh
ng g s services/todo-graphql --skipTests=true
```
in `components/todo-graphql.service.ts`
```ts
import { Injectable } from '@angular/core';
import { ApolloQueryResult } from '@apollo/client/core';
import { Apollo } from 'apollo-angular';
import gql from 'graphql-tag';
import { BehaviorSubject, Observable } from 'rxjs';
import { first, map, switchMap, tap } from 'rxjs/operators';
import { TodoInput } from '../models/todo-input.model';
import { TodoPayload } from '../models/todo-payload.model';

const GET_TODOS = gql`
  {
    todos {
      id,
      title,
      description,
      created,
      updated,
      isDone
    }
  }
`

const GET_TODO = gql`
  query($id: Int) {
    todos(where: {id: {eq: $id}}) {
      id,
      title,
      description,
      created,
      updated,
      isDone
    }
  }
`

const ADD_TODO = gql`
  mutation($input: AddTodoInput) {
    addTodo(input: $input) {
      todo {
        id,
        title,
        description,
        created,
        updated,
        isDone
      }
    }
  }
`

const SET_TODO = gql`
  mutation($input: SetTodoInput) {
    setTodo(input: $input) {
      todo {
        id,
        title,
        description,
        created,
        updated
        isDone
      }
    }
  }
`

const REMOVE_TODO = gql`
  mutation($input: RemoveTodoInput) {
    removeTodo(input: $input) {
      todo {
        id,
        title,
        description,
        created,
        updated,
        isDone
      }
    }
  }
`

interface QueryPayload {
  todos: TodoPayload[]
}
interface MutationPayload {
  data: {
    addTodo: {
      __typename?: string
      todo: TodoPayload
    }
  }
}

@Injectable({
  providedIn: 'root'
})
export class TodoGraphqlService {
  private todos$: BehaviorSubject<TodoPayload[]> = new BehaviorSubject<TodoPayload[]>([])

  constructor(private apollo: Apollo) { }

  getTodos = (cache: boolean): Observable<TodoPayload[]> =>
    this.todos$.value && cache ?
      this.todos$.asObservable() :
      this.apollo.watchQuery<QueryPayload>({ query: GET_TODOS })
        .valueChanges.pipe(
          // map((result: ApolloQueryResult<ApolloPayload>) => result.data.todos)
          switchMap((result: ApolloQueryResult<QueryPayload>): BehaviorSubject<TodoPayload[]> => {
            this.todos$.next(result.data.todos)
            return this.todos$
          })
        )

  getTodo = (id: number): Observable<TodoPayload> => 
    this.apollo.watchQuery<QueryPayload>({ query: GET_TODO, variables: { id: id }})
      .valueChanges.pipe(first(), map((result: ApolloQueryResult<QueryPayload>) => result.data.todos[0]))

  addTodo = (payload: TodoInput): Observable<MutationPayload> =>
    this.apollo.mutate<MutationPayload>({ mutation: ADD_TODO, variables: { input: payload } })
      .pipe(
        first(),
        tap((response: any): void => {
          if (response.data) {
            const values: TodoPayload[] = [...this.todos$.value, response.data.addTodo.todo]
            this.todos$.next(values)
          }
        })
      )

  setTodo = (id: number, payload: TodoInput): Observable<MutationPayload> =>
    this.apollo.mutate<MutationPayload>({ mutation: SET_TODO, 
      variables: { 
        input: { 
          id: id,
          title: payload.title,
          description: payload.description,
          isDone: payload.isDone
        }
      }
    })
    .pipe(
      first(),
      tap((response: any): void => {
        const values: TodoPayload[] = [...this.todos$.value]
        const valueIndex: number = values.findIndex((item: TodoPayload) => item.id === item.id)
        values[valueIndex] = response.data.addTodo.todo
        this.todos$.next(values)
      })
    )

  removeTodo = (id: number): Observable<MutationPayload> =>
    this.apollo.mutate<MutationPayload>({ mutation: REMOVE_TODO, variables: { input: { id: id } } })
      .pipe(
        first(),
        tap((response: any): void => {
          const values: TodoPayload[] = this.todos$.value.filter(value => value.id !== id)
          this.todos$.next(values)
        })
      )
  
}
```
### Create component
in `shell`
```sh
ng g c components/todo-graphql --skipTests=true --module ./modules/shared
```
in `modules/shared.module.ts`
```ts
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TodoComponent } from '../components/todo/todo.component';
import { RouterModule } from '@angular/router';
import { TodoGraphqlComponent } from '../components/todo-graphql/todo-graphql.component';



@NgModule({
  declarations: [
    TodoComponent,
    TodoGraphqlComponent
  ],
  imports: [
    CommonModule,
    RouterModule
  ],
  exports: [
    TodoComponent,
    // ADD THIS
    TodoGraphqlComponent
  ]
})
export class SharedModule { }

```
in `components/todo-graphql/todo-graphql.component.html
```ts
import { Component, OnInit } from '@angular/core';
import { Apollo, gql } from 'apollo-angular';
import { Observable } from 'rxjs';
import { TodoInput } from 'src/app/models/todo-input.model';
import { TodoPayload } from 'src/app/models/todo-payload.model';
import { TodoGraphqlService } from 'src/app/services/todo-graphql.service';

@Component({
  selector: 'app-todo-graphql',
  templateUrl: './todo-graphql.component.html',
  styleUrls: ['./todo-graphql.component.scss']
})
export class TodoGraphqlComponent implements OnInit {

  todos$: Observable<TodoPayload[]> = new Observable();

  todo$: Observable<TodoPayload> = new Observable();

  constructor(private service: TodoGraphqlService) { }

  ngOnInit() {
    this.getTodos(false)
  }

  getTodos(cache: boolean): void {
    this.todos$ = this.service.getTodos(cache)
  }

  getTodo(id: number): void {
    this.todo$ = this.service.getTodo(id)
  }

  addTodo(): void {
    this.service.addTodo({ title: "new brand title", description: "new brand description", isDone: true })
      .subscribe(_ => this.getTodos(true))
  }

  setTodo(id: number, payload: TodoInput = { title: "heh", description: "changed", isDone: false }): void {
    this.service.setTodo(id, payload)
      .subscribe(_ => this.getTodos(true))
  }

  removeTodo(id: number): void {
    this.service.removeTodo(id)
      .subscribe(_ => this.getTodos(true))
  }
}
```
in `components/todo-graphql/todo-graphql.component.html`
```html
<h2>todo-graphql</h2>
<h3 (click)="addTodo()">
    Add me baby
</h3>
<div *ngFor="let todo of todos$ | async">
    <p (click)="getTodo(todo.id)">
        {{ todo }}
    </p>
    <p (click)="setTodo(todo.id)" style="color:green">
        edit me pls
    </p>
    <p (click)="removeTodo(todo.id)" style="color:red">
        remove me pls
    </p>
</div>
<h3>
    Single
</h3>
<p>
    {{ todo$ | async | json}}
</p>
```
