### Add env variables
in `src/environments/environment.ts`
```ts
// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  apiUrl: 'https://jsonplaceholder.typicode.com/',
  production: false
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
```
in `src/app/components/home/home.component.ts`
```ts
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
// ADD THIS
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent {

  constructor(private router: Router) { }
  
  // ADD THIS
  getEnvVariable(): void {
    console.log(environment.apiUrl)
  }
}
```
in `src/app/components/home/home.component.html`
```html
<p routerLink="/">home works!</p>
<p (click)="getEnvVariable()">click me</p>
```
