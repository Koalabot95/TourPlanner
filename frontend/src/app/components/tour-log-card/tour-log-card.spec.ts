import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TourLogCard } from './tour-log-card';

describe('LogCard', () => {
  let component: TourLogCard;
  let fixture: ComponentFixture<TourLogCard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TourLogCard],
    }).compileComponents();

    fixture = TestBed.createComponent(TourLogCard);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
