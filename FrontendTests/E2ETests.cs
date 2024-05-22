// <copyright file="E2ETests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace FrontendTests;

using Microsoft.Playwright;

/// <summary>
/// Frontend end-to-end tests.
/// </summary>
[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class E2ETests : PageTest
{
    /// <summary>
    /// Login test.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Order(1)]
    [Test]
    public async Task LoginTest()
    {
        await this.Login(this.Page);
    }

    /// <summary>
    /// Logout test.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    [Order(2)]
    [Test]
    public async Task LogoutTest()
    {
        await this.Login(this.Page);

        await this.Page.Locator(".menu-button").ClickAsync();
        await this.Page.Locator(".logout-button").ClickAsync();
        await this.Expect(this.Page).ToHaveURLAsync("http://localhost:8000/login");
    }

    /// <summary>
    /// Add exam test.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    [Order(3)]
    [Test]
    public async Task AddExamTest()
    {
        await this.Login(this.Page);

        await this.Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { NameString = "Добавить" }).ClickAsync();
        await this.Page.GetByRole(AriaRole.Dialog).Locator(".student-initials-input input").First.ClickAsync();
        await this.Page.GetByRole(AriaRole.Dialog).Locator(".student-initials-input input").First
            .FillAsync("Студент Студентов Студентович");
        await this.Page.GetByRole(AriaRole.Dialog).Locator(".student-group-input input").ClickAsync();
        await this.Page.GetByRole(AriaRole.Dialog).Locator(".student-group-input input").FillAsync("22.Б22");
        await this.Page.GetByRole(AriaRole.Dialog).Locator(".exam-title-input input").ClickAsync();
        await this.Page.GetByRole(AriaRole.Dialog).Locator(".exam-title-input input").FillAsync("Дисциплина");
        await this.Page.Locator(".lecturer-initials-input input").ClickAsync();
        await this.Page.Locator(".lecturer-initials-input input").FillAsync("Лекторов Лектор Лекторович");
        await this.Page.Locator(".exam-type-input").ClickAsync();
        await this.Page.Locator("li[data-value=\"Сдача\"]").ClickAsync();
        await this.Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { NameString = "Choose date" })
            .ClickAsync();
        await this.Page.GetByRole(AriaRole.Gridcell, new PageGetByRoleOptions { NameString = "13" }).ClickAsync();
        await this.Page.GetByRole(AriaRole.Option, new PageGetByRoleOptions { NameString = "12 hours" }).ClickAsync();
        await this.Page.GetByRole(AriaRole.Option, new PageGetByRoleOptions { NameString = "0 minutes" }).ClickAsync();
        await this.Page.Locator(".location-input input").ClickAsync();
        await this.Page.Locator(".location-input input").FillAsync("3389");
        this.Page.Dialog += PageDialog1EventHandler;
        await this.Page.Locator(".submit-button").ClickAsync();
        await this.Expect(this.Page).ToHaveURLAsync("http://localhost:8000/");
        return;

        void PageDialog1EventHandler(object? sender, IDialog dialog)
        {
            Assert.That(dialog.Message, Is.EqualTo("Запись успешно добавлена"));
            Console.WriteLine($"Dialog message: {dialog.Message}");
            dialog.DismissAsync();
            this.Page.Dialog -= PageDialog1EventHandler;
        }
    }

    /// <summary>
    /// Switch status test.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    [Order(4)]
    [Test]
    public async Task SwitchStatusTest()
    {
        await this.Login(this.Page);

        var unpassedTableRow = this.Page.Locator("tbody").First.Locator("tr").Last;
        await unpassedTableRow.Locator("button").First.ClickAsync();

        await this.Page.Locator(".show-passed-button").ClickAsync();

        var passedTableRow = this.Page.Locator("tbody").Nth(1).Locator("tr").Last;
        await passedTableRow.Locator("button").First.ClickAsync();
    }

    /// <summary>
    /// Delete exam test.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    [Order(5)]
    [Test]
    public async Task DeleteExamTest()
    {
        await this.Login(this.Page);

        var unpassedTableRow = this.Page.Locator("tbody").First.Locator("tr").Last;
        await unpassedTableRow.Locator(".delete-button").ClickAsync();
    }

    private async Task Login(IPage page)
    {
        await page.GotoAsync("http://localhost:8000/");
        await this.Expect(page).ToHaveURLAsync("http://localhost:8000/login");
        await page.Locator(".login-input").ClickAsync();
        await page.Locator(".login-input input").FillAsync("admin");
        await page.Locator(".password-input").ClickAsync();
        await page.Locator(".password-input input").FillAsync("admin");
        await page.Locator(".submit-button").ClickAsync();
        await this.Expect(page).ToHaveURLAsync("http://localhost:8000/");
    }
}