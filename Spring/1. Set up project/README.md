# Set up project
* Generate project
* Uncomment dependency (for now)
* Build project and make first endpoint

### Generate project
Visit this [link](https://start.spring.io) and generate project like in image below and choose your version of java\
\
![Screenshot 2021-02-05 at 08 46 39](https://user-images.githubusercontent.com/53497782/107004568-c6027980-678e-11eb-8c0e-206ad63b27bf.png)
\
\
### Uncomment dependency (for now)
Uncomment postgresql dependecy from pom.xml file and reload project clicking button marked on image below or clicking directly on pom.xml -> maven -> reload project
\
![Screenshot 2021-02-05 at 08 51 00](https://user-images.githubusercontent.com/53497782/107005636-42e22300-6790-11eb-91db-8d41b8a8308a.png)
\
\
### Build project and make first endpoint
Visit your
\
```java
package com.example.demo;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;

import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;
import java.util.Map;
import java.util.LinkedHashMap;
 
@SpringBootApplication
@RestController
public class DemoApplication {

    public static void main(String[] args) { SpringApplication.run(DemoApplication.class, args); }

    @GetMapping("hi")
    public Map<String, Object> hi() {
        Map<String, Object> rtn = new LinkedHashMap<>();
        rtn.put("pic", "hi");
        rtn.put("potato", "King Potato");
        return rtn;
    }
}
```
