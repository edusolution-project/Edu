import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentImportComponent } from './student-import.component';

describe('StudentImportComponent', () => {
  let component: StudentImportComponent;
  let fixture: ComponentFixture<StudentImportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StudentImportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StudentImportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
