import {test, expect, Page} from '@playwright/test';

const login = async (page: Page) => {
    await page.goto("/")
    await expect(page).toHaveURL("/login")
    await page.locator(".login-input").click();
    await page.locator(".login-input input").fill("admin");
    await page.locator(".password-input").click();
    await page.locator(".password-input input").fill("admin");
    await page.locator(".submit-button").click();
    await expect(page).toHaveURL("/");
}

test('login', async ({page}) => {
    await login(page)
});

test('logout', async ({page}) => {
    await login(page)

    await page.locator(".menu-button").click();
    await page.locator(".logout-button").click();
    await expect(page).toHaveURL("/login");
});

test('add exam', async ({page}) => {
    await login(page)

    await page.getByRole("button", {name: "Добавить"}).click();
    await page.getByRole("dialog").locator(".student-initials-input input").first().click();
    await page.getByRole("dialog").locator(".student-initials-input input").first()
        .fill("Студент Студентов Студентович");
    await page.getByRole("dialog").locator(".student-group-input input").click();
    await page.getByRole("dialog").locator(".student-group-input input").fill("22.Б22");
    await page.getByRole("dialog").locator(".exam-title-input input").click();
    await page.getByRole("dialog").locator(".exam-title-input input").fill("Дисциплина");
    await page.locator(".lecturer-initials-input input").click();
    await page.locator(".lecturer-initials-input input").fill("Лекторов Лектор Лекторович");
    await page.locator(".exam-type-input").click();
    await page.locator("li[data-value=\"Сдача\"]").click();
    await page.getByRole("button", {name: "Choose date"}).click();
    await page.getByRole("gridcell", {name: "13"}).click();
    await page.getByRole("option", {name: "12 hours"}).click();
    await page.getByRole("option", {name: "0 minutes"}).first().click();
    await page.locator(".location-input input").click();
    await page.locator(".location-input input").fill("3389");
    page.once('dialog', dialog => {
        console.log(`Dialog message: ${dialog.message()}`);
        dialog.dismiss().catch(() => {
        });
    });
    await page.locator(".submit-button").click();

    await expect(page).toHaveURL("/");
});

test('switch status', async ({page}) => {
    await login(page)

    const unpassedTableRow = page.locator("tbody").first().locator("tr").last();
    await unpassedTableRow.locator("button").first().click();

    await page.locator(".show-passed-button").click();

    const passedTableRow = page.locator("tbody").nth(1).locator("tr").last();
    await passedTableRow.locator("button").first().click();
});

test('delete exam', async ({page}) => {
    await login(page)

    const unpassedTableRow = page.locator("tbody").first().locator("tr").last();
    await unpassedTableRow.locator(".delete-button").click();
});