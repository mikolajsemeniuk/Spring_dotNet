# Simple CRUD
* Create controller

### Create controller

```java
package com.example.demo.student;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping(path = "api/v1")
public class StudentController {

    private final StudentService studentService;

    @Autowired
    public StudentController(StudentService studentService) {
        this.studentService = studentService;
    }

    @GetMapping("")
    public String hello() {
        return "Hello there";
    }

    @GetMapping("students")
    public List<Student> students() {
        return studentService.getStudents();
    }

    @PostMapping("students")
    public void registerNewStudent(@RequestBody Student student) {
        studentService.addNewStudent(student);
    }

//    @GetMapping(path = "/usr/{userId}")
//    @RequestMapping(value = "/usr/{userId}", method = RequestMethod.GET)

    @DeleteMapping(path = "students/{studentId}")
    public void deleteStudent(@PathVariable("studentId") Long id) {
        studentService.deleteStudent(id);
    }

    @PutMapping(path = "students/{studentId}")
    public void updateStudent(@PathVariable("studentId") Long studentId, @RequestParam(required = false) String name, @RequestParam(required = false) String email) {
        studentService.updateStudent(studentId, name, email);
    }

//    @GetMapping("hi")
//    public Map<String, Object> hi() {
//        Map<String, Object> rtn = new LinkedHashMap<>();
//        rtn.put("pic", "hi");
//        rtn.put("potato", "King Potato");
//        return rtn;
//    }

}

```
