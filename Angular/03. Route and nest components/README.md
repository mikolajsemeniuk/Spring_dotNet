# Components
* Route
* Navigate
### Route
In `app-routing.module.ts`
```ts
const routes: Routes = [
  { path: 'home', component: HomeComponent },
  { path: 'home/:id', component: HomeComponent },
  { path: 'children', children: [
      { path: '', component: HomeComponent },
      { path: 'one', component: HomeComponent },
      { path: 'two', component: HomeComponent }
    ]
  },
  { path: 'redirect', redirectTo: 'home' },
  { path: '**', redirectTo: 'home' },
];
```
### Navigate
if your component is **NOT IN APP MODULE** then add this to your module component
```ts
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HomeComponent } from '../components/home/home.component';
// ADD THIS
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [
    HomeComponent
  ],
  imports: [
    CommonModule,
    // ADD THIS
    RouterModule
  ],
  exports: [
    HomeComponent
  ]
})
export class SharedModule { }
```
and then in `src/app/components/home.component.html`
```html
<p routerLink="/">home works!</p>
```
