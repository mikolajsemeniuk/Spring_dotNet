# Simple CRUD
* Create controller
* Create BodyClass

### Create controller

```java
package com.JAPI.controllers;

import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.List;

@RestController
@RequestMapping(value = "users")
public class UsersController {

    private List<Integer> users = new ArrayList<>();

    public UsersController() {
        users.addAll(Arrays.asList(1, 2, 3));
    }

    // Simple get
    @GetMapping
    public ResponseEntity get() {
        return new ResponseEntity(
            new HashMap<String, Object>() {{
                put("message", "get");
                put("data", users);
            }},
            HttpStatus.OK
        );
    }

    // Getting from Path variable
    @PutMapping(value = "{index}/{value}")
    public ResponseEntity getById(@PathVariable Integer index, @PathVariable Integer value) {
        users.add(index, value);
        return new ResponseEntity(new HashMap<String, Object>() {{
            put("message", "put");
            put("data", users);
        }}, HttpStatus.CREATED);
    }

    // Getting from query params
    @PatchMapping
    public ResponseEntity update(@RequestParam(name = "index") Integer index, @RequestParam(name = "value") Integer value) {
        users.add(index, value);
        return new ResponseEntity(new HashMap<String, Object>() {{
            put("message", "patch");
            put("data", users);
        }}, HttpStatus.CREATED);
    }

    // Getting from Body
    @PostMapping
    public ResponseEntity post(@RequestBody SampleBody sampleBody) {
        users.add(sampleBody.index, sampleBody.value);
        return new ResponseEntity(new HashMap<String, Object>() {{
            put("message", "post");
            put("data", users);
        }}, HttpStatus.CREATED);
    }
}
```
### Create BodyClass
```java
package com.JAPI.controllers;

public class SampleBody {
    public Integer index;
    public Integer value;
}

```
