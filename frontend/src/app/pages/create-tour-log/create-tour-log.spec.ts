import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CreateTourLog } from './create-tour-log';

describe('CreateTourLog', () => {
  let component: CreateTourLog;
  let fixture: ComponentFixture<CreateTourLog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateTourLog],
    }).compileComponents();

    fixture = TestBed.createComponent(CreateTourLog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
