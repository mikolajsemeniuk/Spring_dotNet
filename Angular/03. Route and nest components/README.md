# Components
* Route components
* Nest components
#### Route components
In `app-routing.module.ts`
```ts
const routes: Routes = [
  { path: 'home', component: HomeComponent }
];
```
#### Nest components
In `app.component.html`
```html
<h1>App component here</h1>
<!-- Nested component -->
<app-home></app-home>

<p routerLink="/info">go to info page</p>
<p [routerLink]="['/info']">go to info page</p>

<router-outlet></router-outlet>
```
