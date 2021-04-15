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
```ts
