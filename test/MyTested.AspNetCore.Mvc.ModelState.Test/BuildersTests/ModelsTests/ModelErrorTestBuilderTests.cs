﻿namespace MyTested.AspNetCore.Mvc.Test.BuildersTests.ModelsTests
{
    using System.Collections.Generic;
    using Exceptions;
    using Setups;
    using Setups.Controllers;
    using Setups.Models;
    using Xunit;

    public class ModelErrorTestBuilderTests
    {
        [Fact]
        public void ContainingNoErrorsShouldNotThrowExceptionWhenThereAreNoModelStateErrors()
        {
            var requestBody = TestObjectFactory.GetValidRequestModel();

            MyController<MvcController>
                .Instance()
                .Calling(c => c.OkResultActionWithRequestBody(1, requestBody))
                .ShouldHave()
                .ModelState(modelState => modelState
                    .ContainingNoErrors())
                .AndAlso()
                .ShouldReturn()
                .Ok(ok => ok
                    .WithModelOfType<ICollection<ResponseModel>>());
        }

        [Fact]
        public void ContainingNoErrorsShouldThrowExceptionWhenThereAreModelStateErrors()
        {
            var requestBodyWithErrors = TestObjectFactory.GetRequestModelWithErrors();

            Test.AssertException<ModelErrorAssertionException>(
                () =>
                {
                    MyController<MvcController>
                        .Instance()
                        .Calling(c => c.OkResultActionWithRequestBody(1, requestBodyWithErrors))
                        .ShouldHave()
                        .ModelState(modelState => modelState
                            .ContainingNoErrors())
                        .AndAlso()
                        .ShouldReturn()
                        .Ok(ok => ok
                            .WithModelOfType<ICollection<ResponseModel>>());
                },
                "When calling OkResultActionWithRequestBody action in MvcController expected to have valid model state with no errors, but it had some.");
        }

        [Fact]
        public void AndModelStateErrorShouldNotThrowExceptionWhenTheProvidedModelStateErrorExists()
        {
            var requestBodyWithErrors = TestObjectFactory.GetRequestModelWithErrors();

            MyController<MvcController>
                .Instance()
                .Calling(c => c.ModelStateCheck(requestBodyWithErrors))
                .ShouldHave()
                .ModelState(modelState => modelState
                    .ContainingError("RequiredString")
                    .AndAlso()
                    .ContainingNoError("MissingError"))
                .AndAlso()
                .ShouldReturn()
                .Ok(ok => ok
                    .WithModel(requestBodyWithErrors));
        }

        [Fact]
        public void AndModelStateErrorShouldThrowExceptionWhenTheProvidedModelStateErrorDoesNotExists()
        {
            Test.AssertException<ModelErrorAssertionException>(
                () =>
                {
                    var requestBodyWithErrors = TestObjectFactory.GetRequestModelWithErrors();

                    MyController<MvcController>
                        .Instance()
                        .Calling(c => c.ModelStateCheck(requestBodyWithErrors))
                        .ShouldHave()
                        .ModelState(modelState => modelState
                            .ContainingNoError("RequiredString"))
                        .AndAlso()
                        .ShouldReturn()
                        .Ok(ok => ok
                            .WithModel(requestBodyWithErrors));
                },
                "When calling ModelStateCheck action in MvcController expected to not have a model error against key 'RequiredString', but in fact such was found.");
        }

        [Fact]
        public void AndModelStateErrorShouldThrowExceptionWhenTheProvidedModelStateErrorDoesNotExist()
        {
            var requestBody = TestObjectFactory.GetValidRequestModel();

            Test.AssertException<ModelErrorAssertionException>(
                () =>
                {
                    MyController<MvcController>
                        .Instance()
                        .Calling(c => c.ModelStateCheck(requestBody))
                        .ShouldHave()
                        .ModelState(modelState => modelState
                            .ContainingError("Name"))
                        .AndAlso()
                        .ShouldReturn()
                        .Ok(ok => ok
                            .WithModel(requestBody));
                },
                "When calling ModelStateCheck action in MvcController expected to have a model error against key 'Name', but in fact none was found.");
        }

        [Fact]
        public void AndProvideTheModelShouldReturnProperModelWhenThereIsResponseModel()
        {
            MyController<MvcController>
                .Instance()
                .Calling(c => c.OkResultWithResponse())
                .ShouldReturn()
                .Ok(ok => ok
                    .WithModelOfType<List<ResponseModel>>()
                    .ShouldPassForThe<List<ResponseModel>>(responseModel =>
                    {
                        Assert.NotNull(responseModel);
                        Assert.IsAssignableFrom<List<ResponseModel>>(responseModel);
                        Assert.Equal(2, responseModel.Count);
                    }));
        }

        [Fact]
        public void AndProvideTheModelShouldReturnProperModelWhenThereIsResponseModelWithPassing()
        {
            MyController<MvcController>
                .Instance()
                .Calling(c => c.OkResultWithResponse())
                .ShouldReturn()
                .Ok(ok => ok
                    .WithModelOfType<List<ResponseModel>>()
                    .Passing(m => m.Count == 2)
                    .ShouldPassForThe<List<ResponseModel>>(responseModel =>
                    {
                        Assert.NotNull(responseModel);
                        Assert.IsAssignableFrom<List<ResponseModel>>(responseModel);
                        Assert.Equal(2, responseModel.Count);
                    }));
        }

        [Fact]
        public void AndProvideTheModelShouldReturnProperModelWhenThereIsResponseModelWithModelStateError()
        {
            MyController<MvcController>
                .Instance()
                .Calling(c => c.CustomModelStateError())
                .ShouldHave()
                .ModelState(modelState => modelState
                    .ContainingError("Test"))
                .AndAlso()
                .ShouldReturn()
                .Ok(ok => ok
                    .WithModelOfType<ICollection<ResponseModel>>()
                    .ShouldPassForThe<ICollection<ResponseModel>>(responseModel =>
                    {
                        Assert.NotNull(responseModel);
                        Assert.IsAssignableFrom<List<ResponseModel>>(responseModel);
                        Assert.Equal(2, responseModel.Count);
                    }));
        }

        [Fact]
        public void AndProvideTheModelShouldReturnProperModelWhenThereIsResponseModelWithModelStateErrorAndErrorCheck()
        {
            MyController<MvcController>
                .Instance()
                .Calling(c => c.CustomModelStateError())
                .ShouldHave()
                .ModelState(modelState => modelState
                    .ContainingError("Test")
                    .ThatEquals("Test error"))
                .AndAlso()
                .ShouldReturn()
                .Ok(ok => ok
                    .WithModelOfType<ICollection<ResponseModel>>()
                    .ShouldPassForThe<ICollection<ResponseModel>>(responseModel =>
                    {
                        Assert.NotNull(responseModel);
                        Assert.IsAssignableFrom<List<ResponseModel>>(responseModel);
                        Assert.Equal(2, responseModel.Count);
                    }));
        }
    }
}
