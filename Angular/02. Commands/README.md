# Commands
* Modules
* Components
* Models
* Services
* Guards

### Modules
Create Module
```sh
ng g m modules/shared # with folder
ng g m modules/shared --flat # without folder
```
in `src/app/modules/shared.module.ts`
```ts
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

@NgModule({
  declarations: [],
  imports: [
    CommonModule
  ],
  // ADD THIS TO ENABLE ALL
  // STAFF IN THIS MODULE TO
  // BE IMPORTED IN `src/app/app.module.ts`
  exports: [
    // PUT ALL COMPONENTS, MODELS, SERVICES HERE
  
  ]
})
export class SharedModule { }
```
in `src/app/app.module.ts`
```ts
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

// ADD THIS
import { SharedModule } from './modules/shared.module';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    // ADD THIS
    SharedModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
```
### Components
Create Component
```sh
ng g c components/home --skipTests=true --module app # global module
ng g c components/home --skipTests=true --module ./modules/shared # your own module
```
