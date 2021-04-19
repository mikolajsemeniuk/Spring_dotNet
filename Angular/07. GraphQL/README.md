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
in `components/todo-graphql.component.ts`
```ts

```
