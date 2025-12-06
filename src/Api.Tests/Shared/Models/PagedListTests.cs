using PropertyManagement.Api.Shared.Models;

namespace PropertyManagement.Api.Tests.Shared.Models;

public sealed class PagedListTests
{
    [Fact]
    public void Create_WithValidData_ReturnsPagedList()
    {
        // Arrange
        var items = new List<string> { "Item1", "Item2", "Item3" };
        var pageNumber = 1;
        var pageSize = 10;
        var totalCount = 25;

        // Act
        var result = PagedList<string>.Create(items, pageNumber, pageSize, totalCount);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(3);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().Be(25);
        result.TotalPages.Should().Be(3);
        result.HasPreviousPage.Should().BeFalse();
        result.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public void TotalPages_CalculatesCorrectly()
    {
        // Arrange & Act
        var result1 = PagedList<string>.Create(new List<string>(), 1, 10, 25);
        var result2 = PagedList<string>.Create(new List<string>(), 1, 10, 30);
        var result3 = PagedList<string>.Create(new List<string>(), 1, 10, 10);

        // Assert
        result1.TotalPages.Should().Be(3);  // 25 / 10 = 2.5 → 3
        result2.TotalPages.Should().Be(3);  // 30 / 10 = 3
        result3.TotalPages.Should().Be(1);  // 10 / 10 = 1
    }

    [Fact]
    public void HasPreviousPage_WhenOnFirstPage_ReturnsFalse()
    {
        // Arrange & Act
        var result = PagedList<string>.Create(new List<string>(), 1, 10, 25);

        // Assert
        result.HasPreviousPage.Should().BeFalse();
    }

    [Fact]
    public void HasPreviousPage_WhenOnSecondPage_ReturnsTrue()
    {
        // Arrange & Act
        var result = PagedList<string>.Create(new List<string>(), 2, 10, 25);

        // Assert
        result.HasPreviousPage.Should().BeTrue();
    }

    [Fact]
    public void HasNextPage_WhenOnLastPage_ReturnsFalse()
    {
        // Arrange & Act
        var result = PagedList<string>.Create(new List<string>(), 3, 10, 25);

        // Assert
        result.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public void HasNextPage_WhenNotOnLastPage_ReturnsTrue()
    {
        // Arrange & Act
        var result = PagedList<string>.Create(new List<string>(), 1, 10, 25);

        // Assert
        result.HasNextPage.Should().BeTrue();
    }
}
