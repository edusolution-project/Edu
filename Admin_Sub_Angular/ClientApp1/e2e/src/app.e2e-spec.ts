// ====================================================
// More Templates: https://www.ebenmonney.com/templates
// Email: support@ebenmonney.com
// ====================================================

import { AppPage } from './app.po';

describe('TestAngular6 App', () => {
  let page: AppPage;

  beforeEach(() => {
    page = new AppPage();
  });

  it('should display application title: TestAngular6', () => {
    page.navigateTo();
    expect(page.getAppTitle()).toEqual('TestAngular6');
  });
});
