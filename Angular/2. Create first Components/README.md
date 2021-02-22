# Components
* Generate Components
* Routes components
* Nest components
* Pass variables and functions
* Emit variables and functions
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

#### Nest components
```html
<h1>App component here</h1>
<!-- Nested component -->
<app-home></app-home>

<router-outlet></router-outlet>
```
