# Components
* Generate Components
* Routes components
* Nest components
#### Generate Components
Basic
```sh
ng generate component components/home
```
Shorter
```sh
ng g c components/home
```
Without tests
```sh
ng g c components/home --skipTests=true
```
#### Route components
In `app-routing.module.ts`
```ts
const routes: Routes = [
  { path: 'info', component: HomeComponent }
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