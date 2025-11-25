using System.Net;
using ReheeCmf.Commons;
using ReheeCmf.Helpers;
using Xunit;

namespace ReheeCmf.Utility.Tests;

public class ContentResponseHelperTests
{
    [Fact]
    public void SetContent_SetsAllProperties()
    {
        var response = new ContentResponse<string>();
        var error = new Error();

        response.SetContent("test content", true, HttpStatusCode.OK, error);

        Assert.Equal("test content", response.Content);
        Assert.True(response.Success);
        Assert.Equal(HttpStatusCode.OK, response.Status);
        Assert.NotNull(response.Errors);
        Assert.Single(response.Errors);
    }

    [Fact]
    public void SetContent_WithNullContent_SetsContentToDefault()
    {
        var response = new ContentResponse<string>();

        response.SetContent(null, true, HttpStatusCode.OK);

        Assert.Null(response.Content);
        Assert.True(response.Success);
        Assert.Equal(HttpStatusCode.OK, response.Status);
    }

    [Fact]
    public void SetContent_WithConvertibleContent_ConvertsContent()
    {
        var response = new ContentResponse<int>();

        response.SetContent(42, true, HttpStatusCode.OK);

        Assert.Equal(42, response.Content);
        Assert.True(response.Success);
    }

    [Fact]
    public void SetContent_WithIncompatibleContent_SetsContentToDefault()
    {
        var response = new ContentResponse<int>();

        response.SetContent("not a number", false, HttpStatusCode.BadRequest);

        Assert.Equal(default, response.Content);
        Assert.False(response.Success);
    }

    [Fact]
    public void SetContent_WithNoErrors_DoesNotSetErrors()
    {
        var response = new ContentResponse<string>();

        response.SetContent("test", true, HttpStatusCode.OK);

        Assert.Null(response.Errors);
    }

    [Fact]
    public void SetSuccess_SetsSuccessToTrueAndStatusToOK()
    {
        var response = new ContentResponse<string>();

        response.SetSuccess("success content");

        Assert.Equal("success content", response.Content);
        Assert.True(response.Success);
        Assert.Equal(HttpStatusCode.OK, response.Status);
        Assert.Null(response.Errors);
    }

    [Fact]
    public void SetSuccess_WithIntegerContent_SetsContent()
    {
        var response = new ContentResponse<int>();

        response.SetSuccess(123);

        Assert.Equal(123, response.Content);
        Assert.True(response.Success);
        Assert.Equal(HttpStatusCode.OK, response.Status);
    }

    [Fact]
    public void SetErrors_SetsSuccessToFalseAndSetsErrors()
    {
        var response = new ContentResponse<string>();
        var error1 = new Error();
        var error2 = new Error();

        response.SetErrors(HttpStatusCode.BadRequest, error1, error2);

        Assert.Null(response.Content);
        Assert.False(response.Success);
        Assert.Equal(HttpStatusCode.BadRequest, response.Status);
        Assert.NotNull(response.Errors);
        Assert.Equal(2, response.Errors.Count());
    }

    [Fact]
    public void SetErrors_WithInternalServerError_SetsCorrectStatus()
    {
        var response = new ContentResponse<object>();
        var error = new Error();

        response.SetErrors(HttpStatusCode.InternalServerError, error);

        Assert.False(response.Success);
        Assert.Equal(HttpStatusCode.InternalServerError, response.Status);
    }

    [Fact]
    public void ObjContent_ReturnsContent()
    {
        var response = new ContentResponse<string>();
        response.SetSuccess("test value");

        object? objContent = response.ObjContent;

        Assert.Equal("test value", objContent);
    }

    [Fact]
    public void ObjContent_WithIntContent_ReturnsContent()
    {
        var response = new ContentResponse<int>();
        response.SetSuccess(42);

        object? objContent = response.ObjContent;

        Assert.Equal(42, objContent);
    }
}
