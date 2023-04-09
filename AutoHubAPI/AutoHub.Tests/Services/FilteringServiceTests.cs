﻿using AutoHub.Core.Contracts;
using AutoHub.Core.FilterDefinitions;
using AutoHub.Core.Services;
using AutoHub.Data.Contracts;
using AutoHub.Data.Models;
using AutoHub.Tests.Randomizers;
using AutoHub.Utilities;
using Moq;

namespace AutoHub.Tests.Services;

public class FilteringServiceTests
{
    /*[Fact]
    public async void GetByWithValidIdReturnsEntity()
    {
        var carRandomizer = new CarRandomizer();
        var expectedEntity = carRandomizer.PrepareRandomValue();

        var mockRepository = new Mock<IRepository<Car>>();
        mockRepository.Setup(r => r.GetAsync(expectedEntity.Id))
            .ReturnsAsync(new OperationResult<Car>(){Data = expectedEntity});

        var service = new Service<Car>(mockRepository.Object);

        var result = await service.GetAsync(expectedEntity.Id);
        
        Assert.True(result.IsSuccessful);
        Assert.Equal(expectedEntity, result.Data);
    }*/

    [Fact]
    public async void OrderByShouldReturnCorrectlyOrderedCollectionAscending()
    {
        var orderDefinition = new OrderDefinition() { Property = "Price", IsAscending = true };

        List<Car> cars = new();

        var carRandomizer = new CarRandomizer();
        for (int i = 0; i < 5; i++) cars.Add(carRandomizer.PrepareRandomValue());
        
        var expected = cars.OrderBy(x => x.Price);

        var mockService = new Mock<IService<Car>>();
        mockService.Setup(s => s.GetManyAsync())
            .ReturnsAsync(new OperationResult<IEnumerable<Car>>() { Data = cars });

        var filteringService = new FilteringService<Car>(mockService.Object);

        var result = await filteringService.OrderBy(orderDefinition);
        
        Assert.True(result.IsSuccessful);
        Assert.Equal(expected, result.Data);
    }

    [Fact]
    public async void OrderByShouldReturnCorrectlyOrderedCollectionDescending()
    {
        var orderDefinition = new OrderDefinition() { Property = "Price", IsAscending = false };

        List<Car> cars = new();

        var carRandomizer = new CarRandomizer();
        for (int i = 0; i < 5; i++) cars.Add(carRandomizer.PrepareRandomValue());
        
        var expected = cars.OrderByDescending(x => x.Price);

        var mockService = new Mock<IService<Car>>();
        mockService.Setup(s => s.GetManyAsync())
            .ReturnsAsync(new OperationResult<IEnumerable<Car>>() { Data = cars });

        var filteringService = new FilteringService<Car>(mockService.Object);

        var result = await filteringService.OrderBy(orderDefinition);
        
        Assert.True(result.IsSuccessful);
        Assert.Equal(expected, result.Data);
    }
}