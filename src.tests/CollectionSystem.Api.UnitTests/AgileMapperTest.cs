using Autofac;
using CollectionSystem.Api.Realization.Bases;
using Shouldly;

namespace CollectionSystem.Api.UnitTests;

public class AgileMapperTest : TestBase
{
    private readonly IAgileMapper agileMapper;

    public AgileMapperTest()
    {
        agileMapper = Container.Resolve<IAgileMapper>();
    }

    [Fact]
    public void ShouldAutoMapperByPropertyName()
    {
        var testEntity = new TestEntity
        {
            TestName = "TestEntityName",
            CreatedOn = DateTimeOffset.Now,
            TestId = Guid.NewGuid()
        };
        var testDto = agileMapper.Map<TestEntity, TestDto>(testEntity);
        testDto.TestName.ShouldBe(testEntity.TestName);
        testDto.CreatedOn.ShouldBe(testEntity.CreatedOn);
        testDto.TestId.ShouldBe(testEntity.TestId);
        testDto.CreatedName.ShouldBe(default);
    }

    [Fact]
    public Task ShouldMapperByValue()
    {
        var testEntity = new TestEntity
        {
            TestName = "TestEntityName",
            CreatedOn = DateTimeOffset.Now,
            TestId = Guid.NewGuid()
        };
        var testDto =
            agileMapper.Map<TestEntity, TestDto>(testEntity,
                e => { e.Profile(x => x.Target.CreatedName, "小明"); });
        testDto.TestName.ShouldBe(testEntity.TestName);
        testDto.CreatedOn.ShouldBe(testEntity.CreatedOn);
        testDto.TestId.ShouldBe(testEntity.TestId);
        testDto.CreatedName.ShouldBe("小明");
        return Task.CompletedTask;
    }


    private class TestEntity
    {
        public Guid TestId { get; set; }

        public string TestName { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
    }

    private class TestDto
    {
        public Guid TestId { get; set; }

        public string TestName { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public string CreatedName { get; set; }
    }
}