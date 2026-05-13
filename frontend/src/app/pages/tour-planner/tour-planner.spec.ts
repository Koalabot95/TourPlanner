import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TourPlanner } from './tour-planner';

describe('TourPlanner', () => {
  let component: TourPlanner;
  let fixture: ComponentFixture<TourPlanner>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TourPlanner],
    }).compileComponents();

    fixture = TestBed.createComponent(TourPlanner);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
