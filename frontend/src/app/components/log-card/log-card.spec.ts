import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LogCard } from './log-card';

describe('LogCard', () => {
  let component: LogCard;
  let fixture: ComponentFixture<LogCard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LogCard],
    }).compileComponents();

    fixture = TestBed.createComponent(LogCard);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
