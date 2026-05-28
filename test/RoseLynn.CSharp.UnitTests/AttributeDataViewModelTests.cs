using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLynn.CSharp;
using RoseLynn.Testing;
using RoseLynn.Core.Utilities;

namespace RoseLynn.Test;

public sealed class AttributeDataViewModelTests
    : BaseCompilationTestContainer
{
    [Test]
    public async Task Empty(CancellationToken cancellationToken)
    {
        //lang=C#
        const string testSource =
            """
            using System;

            namespace XmlDocFormat.AttributeDataViewModelTests;

            public sealed class EmptyAttribute : Attribute;

            [Empty]
            public sealed class User;
            """;

        await VerifyCompilation(testSource, Verify, cancellationToken);

        static async Task Verify(
            Compilation compilation,
            CancellationToken cancellationToken)
        {
            var userClassSymbol = DeclaredSymbolOnNode(
                compilation, "class User", cancellationToken)!;
            var model = SingleAttributeDataViewModel(
                userClassSymbol, cancellationToken);

            await Assert.That(model).IsNotNull();
            await Assert.That(model.NamedArguments).IsEmpty();
            await Assert.That(model.ConstructorArguments).IsEmpty();
        }
    }

    [Test]
    public async Task OneConstructorArgument(CancellationToken cancellationToken)
    {
        //lang=C#
        const string testSource =
            """
            using System;

            namespace XmlDocFormat.AttributeDataViewModelTests;

            public sealed class EmptyAttribute(int a) : Attribute;

            [Empty(1)]
            public sealed class User;
            """;

        await VerifyCompilation(testSource, Verify, cancellationToken);

        static async Task Verify(
            Compilation compilation,
            CancellationToken cancellationToken)
        {
            var userClassSymbol = DeclaredSymbolOnNode(
                compilation, "class User", cancellationToken)!;
            var model = SingleAttributeDataViewModel(
                userClassSymbol, cancellationToken);

            await Assert.That(model).IsNotNull();
            var constructor = model.AttributeData.AttributeConstructor;
            await Assert.That(constructor).IsNotNull();
            var constructorParameter = constructor.Parameters.Single();

            await Assert.That(model.NamedArguments).IsEmpty();

            var singleArgument = await Assert.That(model.ConstructorArguments).HasSingleItem();
            await Assert.That(singleArgument.Name).IsEqualTo(constructorParameter.Name);
            await Assert.That(singleArgument.MappingKind)
                .IsEqualTo(AttributeDataViewModel.AttributeArgumentMappingKind.Parameter);
            await Assert.That(singleArgument.Value)
                .HasProperty(s => s.Kind, TypedConstantKind.Primitive)
                .And.HasProperty(s => s.Value, 1)
                .And.HasProperty(s => s.Type!.SpecialType, SpecialType.System_Int32);
        }
    }

    [Test]
    public async Task OneNamedConstructorArgument(CancellationToken cancellationToken)
    {
        //lang=C#
        const string testSource =
            """
            using System;

            namespace XmlDocFormat.AttributeDataViewModelTests;

            public sealed class EmptyAttribute(int a) : Attribute;

            [Empty(a: 1)]
            public sealed class User;
            """;

        await VerifyCompilation(testSource, Verify, cancellationToken);

        static async Task Verify(
            Compilation compilation,
            CancellationToken cancellationToken)
        {
            var userClassSymbol = DeclaredSymbolOnNode(
                compilation, "class User", cancellationToken)!;
            var model = SingleAttributeDataViewModel(
                userClassSymbol, cancellationToken);

            await Assert.That(model).IsNotNull();
            var constructor = model.AttributeData.AttributeConstructor;
            await Assert.That(constructor).IsNotNull();
            var constructorParameter = constructor.Parameters.Single();

            await Assert.That(model.NamedArguments).IsEmpty();

            var singleArgument = await Assert.That(model.ConstructorArguments).HasSingleItem();
            await Assert.That(singleArgument.Name).IsEqualTo(constructorParameter.Name);
            await Assert.That(singleArgument.MappingKind)
                .IsEqualTo(AttributeDataViewModel.AttributeArgumentMappingKind.Parameter);
            await Assert.That(singleArgument.Value)
                .HasProperty(s => s.Kind, TypedConstantKind.Primitive)
                .And.HasProperty(s => s.Value, 1)
                .And.HasProperty(s => s.Type!.SpecialType, SpecialType.System_Int32);
        }
    }

    [Test]
    public async Task OneNamedArgument(CancellationToken cancellationToken)
    {
        //lang=C#
        const string testSource =
            """
            using System;

            namespace XmlDocFormat.AttributeDataViewModelTests;

            public sealed class EmptyAttribute : Attribute
            {
                public string? Preamble { get; set; }
            }

            [Empty(Preamble = "Something")]
            public sealed class User;
            """;

        await VerifyCompilation(testSource, Verify, cancellationToken);

        static async Task Verify(
            Compilation compilation,
            CancellationToken cancellationToken)
        {
            var userClassSymbol = DeclaredSymbolOnNode(
                compilation, "class User", cancellationToken)!;
            var model = SingleAttributeDataViewModel(
                userClassSymbol, cancellationToken);

            await Assert.That(model).IsNotNull();
            var attributeClass = model.AttributeData.AttributeClass;
            await Assert.That(attributeClass).IsNotNull();
            var preambleProperty = attributeClass.GetMembers<IPropertySymbol>().Single();

            await Assert.That(model.ConstructorArguments).IsEmpty();

            var singleArgument = await Assert.That(model.NamedArguments).HasSingleItem();
            await Assert.That(singleArgument.Name).IsEqualTo(preambleProperty.Name);
            await Assert.That(singleArgument.MappingKind)
                .IsEqualTo(AttributeDataViewModel.AttributeArgumentMappingKind.Named);
            await Assert.That(singleArgument.Value)
                .HasProperty(s => s.Kind, TypedConstantKind.Primitive)
                .And.HasProperty(s => s.Value, "Something")
                .And.HasProperty(s => s.Type!.SpecialType, SpecialType.System_String);
        }
    }

    [Test]
    public async Task TwoConstructorOneNamedArguments(CancellationToken cancellationToken)
    {
        //lang=C#
        const string testSource =
            """
            using System;

            namespace XmlDocFormat.AttributeDataViewModelTests;

            public sealed class EmptyAttribute(int a, int b) : Attribute
            {
                public string? Preamble { get; set; }
            }

            [Empty(1, 2, Preamble = "Something")]
            public sealed class User;
            """;

        await VerifyCompilation(testSource, Verify, cancellationToken);

        static async Task Verify(
            Compilation compilation,
            CancellationToken cancellationToken)
        {
            var userClassSymbol = DeclaredSymbolOnNode(
                compilation, "class User", cancellationToken)!;
            var model = SingleAttributeDataViewModel(
                userClassSymbol, cancellationToken);

            await Assert.That(model).IsNotNull();

            var constructor = model.AttributeData.AttributeConstructor;
            await Assert.That(constructor).IsNotNull();
            var firstConstructorParameter = constructor.Parameters[0];
            var secondConstructorParameter = constructor.Parameters[1];

            var attributeClass = model.AttributeData.AttributeClass;
            await Assert.That(attributeClass).IsNotNull();
            var preambleProperty = attributeClass.GetMembers<IPropertySymbol>().Single();

            await Assert.That(model.ConstructorArguments).Count().IsEqualTo(2);
            var firstConstructorArgument = model.ConstructorArguments[0];
            var secondConstructorArgument = model.ConstructorArguments[1];
            await Assert.That(firstConstructorArgument.Name).IsEqualTo(firstConstructorParameter.Name);
            await Assert.That(secondConstructorArgument.Name).IsEqualTo(secondConstructorParameter.Name);

            await Assert.That(firstConstructorArgument.MappingKind)
                .IsEqualTo(AttributeDataViewModel.AttributeArgumentMappingKind.Parameter);
            await Assert.That(secondConstructorArgument.MappingKind)
                .IsEqualTo(AttributeDataViewModel.AttributeArgumentMappingKind.Parameter);

            await Assert.That(firstConstructorArgument.Value)
                .HasProperty(s => s.Kind, TypedConstantKind.Primitive)
                .And.HasProperty(s => s.Value, 1)
                .And.HasProperty(s => s.Type!.SpecialType, SpecialType.System_Int32);
            await Assert.That(secondConstructorArgument.Value)
                .HasProperty(s => s.Kind, TypedConstantKind.Primitive)
                .And.HasProperty(s => s.Value, 2)
                .And.HasProperty(s => s.Type!.SpecialType, SpecialType.System_Int32);

            var singleNamedArgument = await Assert.That(model.NamedArguments).HasSingleItem();
            await Assert.That(singleNamedArgument.Name).IsEqualTo(preambleProperty.Name);
            await Assert.That(singleNamedArgument.MappingKind)
                .IsEqualTo(AttributeDataViewModel.AttributeArgumentMappingKind.Named);
            await Assert.That(singleNamedArgument.Value)
                .HasProperty(s => s.Kind, TypedConstantKind.Primitive)
                .And.HasProperty(s => s.Value, "Something")
                .And.HasProperty(s => s.Type!.SpecialType, SpecialType.System_String);
        }
    }

    [Test]
    public async Task TwoNamedConstructorOneNamedArguments(CancellationToken cancellationToken)
    {
        //lang=C#
        const string testSource =
            """
            using System;

            namespace XmlDocFormat.AttributeDataViewModelTests;

            public sealed class EmptyAttribute(int a, int b) : Attribute
            {
                public string? Preamble { get; set; }
            }

            [Empty(a: 1, b: 2, Preamble = "Something")]
            public sealed class User;
            """;

        await VerifyCompilation(testSource, Verify, cancellationToken);

        static async Task Verify(
            Compilation compilation,
            CancellationToken cancellationToken)
        {
            var userClassSymbol = DeclaredSymbolOnNode(
                compilation, "class User", cancellationToken)!;
            var model = SingleAttributeDataViewModel(
                userClassSymbol, cancellationToken);

            await Assert.That(model).IsNotNull();

            var constructor = model.AttributeData.AttributeConstructor;
            await Assert.That(constructor).IsNotNull();
            var firstConstructorParameter = constructor.Parameters[0];
            var secondConstructorParameter = constructor.Parameters[1];

            var attributeClass = model.AttributeData.AttributeClass;
            await Assert.That(attributeClass).IsNotNull();
            var preambleProperty = attributeClass.GetMembers<IPropertySymbol>().Single();

            await Assert.That(model.ConstructorArguments).Count().IsEqualTo(2);
            var firstConstructorArgument = model.ConstructorArguments[0];
            var secondConstructorArgument = model.ConstructorArguments[1];
            await Assert.That(firstConstructorArgument.Name).IsEqualTo(firstConstructorParameter.Name);
            await Assert.That(secondConstructorArgument.Name).IsEqualTo(secondConstructorParameter.Name);

            await Assert.That(firstConstructorArgument.MappingKind)
                .IsEqualTo(AttributeDataViewModel.AttributeArgumentMappingKind.Parameter);
            await Assert.That(secondConstructorArgument.MappingKind)
                .IsEqualTo(AttributeDataViewModel.AttributeArgumentMappingKind.Parameter);

            await Assert.That(firstConstructorArgument.Value)
                .HasProperty(s => s.Kind, TypedConstantKind.Primitive)
                .And.HasProperty(s => s.Value, 1)
                .And.HasProperty(s => s.Type!.SpecialType, SpecialType.System_Int32);
            await Assert.That(secondConstructorArgument.Value)
                .HasProperty(s => s.Kind, TypedConstantKind.Primitive)
                .And.HasProperty(s => s.Value, 2)
                .And.HasProperty(s => s.Type!.SpecialType, SpecialType.System_Int32);

            var singleNamedArgument = await Assert.That(model.NamedArguments).HasSingleItem();
            await Assert.That(singleNamedArgument.Name).IsEqualTo(preambleProperty.Name);
            await Assert.That(singleNamedArgument.MappingKind)
                .IsEqualTo(AttributeDataViewModel.AttributeArgumentMappingKind.Named);
            await Assert.That(singleNamedArgument.Value)
                .HasProperty(s => s.Kind, TypedConstantKind.Primitive)
                .And.HasProperty(s => s.Value, "Something")
                .And.HasProperty(s => s.Type!.SpecialType, SpecialType.System_String);
        }
    }

    [Test]
    public async Task TwoUnorderedNamedConstructorOneNamedArguments(CancellationToken cancellationToken)
    {
        //lang=C#
        const string testSource =
            """
            using System;

            namespace XmlDocFormat.AttributeDataViewModelTests;

            public sealed class EmptyAttribute(int a, int b) : Attribute
            {
                public string? Preamble { get; set; }
            }

            [Empty(b: 1, a: 2, Preamble = "Something")]
            public sealed class User;
            """;

        await VerifyCompilation(testSource, Verify, cancellationToken);

        static async Task Verify(
            Compilation compilation,
            CancellationToken cancellationToken)
        {
            var userClassSymbol = DeclaredSymbolOnNode(
                compilation, "class User", cancellationToken)!;
            var model = SingleAttributeDataViewModel(
                userClassSymbol, cancellationToken);

            await Assert.That(model).IsNotNull();

            var constructor = model.AttributeData.AttributeConstructor;
            await Assert.That(constructor).IsNotNull();
            var firstConstructorParameter = constructor.Parameters[0];
            var secondConstructorParameter = constructor.Parameters[1];

            var attributeClass = model.AttributeData.AttributeClass;
            await Assert.That(attributeClass).IsNotNull();
            var preambleProperty = attributeClass.GetMembers<IPropertySymbol>().Single();

            await Assert.That(model.ConstructorArguments).Count().IsEqualTo(2);
            var firstConstructorArgument = model.ConstructorArguments[0];
            var secondConstructorArgument = model.ConstructorArguments[1];
            await Assert.That(firstConstructorArgument.Name).IsEqualTo(secondConstructorParameter.Name);
            await Assert.That(secondConstructorArgument.Name).IsEqualTo(firstConstructorParameter.Name);

            await Assert.That(firstConstructorArgument.MappingKind)
                .IsEqualTo(AttributeDataViewModel.AttributeArgumentMappingKind.Parameter);
            await Assert.That(secondConstructorArgument.MappingKind)
                .IsEqualTo(AttributeDataViewModel.AttributeArgumentMappingKind.Parameter);

            await Assert.That(firstConstructorArgument.Value)
                .HasProperty(s => s.Kind, TypedConstantKind.Primitive)
                .And.HasProperty(s => s.Value, 1)
                .And.HasProperty(s => s.Type!.SpecialType, SpecialType.System_Int32);
            await Assert.That(secondConstructorArgument.Value)
                .HasProperty(s => s.Kind, TypedConstantKind.Primitive)
                .And.HasProperty(s => s.Value, 2)
                .And.HasProperty(s => s.Type!.SpecialType, SpecialType.System_Int32);

            var singleNamedArgument = await Assert.That(model.NamedArguments).HasSingleItem();
            await Assert.That(singleNamedArgument.Name).IsEqualTo(preambleProperty.Name);
            await Assert.That(singleNamedArgument.MappingKind)
                .IsEqualTo(AttributeDataViewModel.AttributeArgumentMappingKind.Named);
            await Assert.That(singleNamedArgument.Value)
                .HasProperty(s => s.Kind, TypedConstantKind.Primitive)
                .And.HasProperty(s => s.Value, "Something")
                .And.HasProperty(s => s.Type!.SpecialType, SpecialType.System_String);
        }
    }

    [Test]
    public async Task OneConstructorDefaultValue(CancellationToken cancellationToken)
    {
        //lang=C#
        const string testSource =
            """
            using System;

            namespace XmlDocFormat.AttributeDataViewModelTests;

            public sealed class EmptyAttribute(int a = 1) : Attribute;

            [Empty]
            public sealed class User;
            """;

        await VerifyCompilation(testSource, Verify, cancellationToken);

        static async Task Verify(
            Compilation compilation,
            CancellationToken cancellationToken)
        {
            var userClassSymbol = DeclaredSymbolOnNode(
                compilation, "class User", cancellationToken)!;
            var model = SingleAttributeDataViewModel(
                userClassSymbol, cancellationToken);

            await Assert.That(model).IsNotNull();

            var constructor = model.AttributeData.AttributeConstructor;
            await Assert.That(constructor).IsNotNull();
            var singleParameter = await Assert.That(constructor.Parameters).HasSingleItem();

            var singleArgument = await Assert.That(model.ConstructorArguments).HasSingleItem();
            await Assert.That(singleArgument.Name).IsEqualTo(singleParameter.Name);
            await Assert.That(singleArgument.MappingKind)
                .IsEqualTo(AttributeDataViewModel.AttributeArgumentMappingKind.ParameterDefaultValue);
            await Assert.That(singleArgument.Value)
                .HasProperty(s => s.Kind, TypedConstantKind.Primitive)
                .And.HasProperty(s => s.Value, 1)
                .And.HasProperty(s => s.Type!.SpecialType, SpecialType.System_Int32);

            await Assert.That(model.NamedArguments).IsEmpty();
        }
    }

    [Test]
    public async Task OneDefaultOneUnorderedDefaultConstructorArguments(CancellationToken cancellationToken)
    {
        //lang=C#
        const string testSource =
            """
            using System;

            namespace XmlDocFormat.AttributeDataViewModelTests;

            public sealed class EmptyAttribute(int a = 1, int b = 2) : Attribute;

            [Empty(b: 3)]
            public sealed class User;
            """;

        await VerifyCompilation(testSource, Verify, cancellationToken);

        static async Task Verify(
            Compilation compilation,
            CancellationToken cancellationToken)
        {
            var userClassSymbol = DeclaredSymbolOnNode(
                compilation, "class User", cancellationToken)!;
            var model = SingleAttributeDataViewModel(
                userClassSymbol, cancellationToken);

            await Assert.That(model).IsNotNull();

            var constructor = model.AttributeData.AttributeConstructor;
            await Assert.That(constructor).IsNotNull();
            var firstConstructorParameter = constructor.Parameters[0];
            var secondConstructorParameter = constructor.Parameters[1];

            await Assert.That(model.ConstructorArguments).Count().IsEqualTo(2);
            var firstConstructorArgument = model.ConstructorArguments[0];
            var secondConstructorArgument = model.ConstructorArguments[1];
            await Assert.That(firstConstructorArgument.Name).IsEqualTo(secondConstructorParameter.Name);
            await Assert.That(secondConstructorArgument.Name).IsEqualTo(firstConstructorParameter.Name);

            await Assert.That(firstConstructorArgument.MappingKind)
                .IsEqualTo(AttributeDataViewModel.AttributeArgumentMappingKind.Parameter);
            await Assert.That(secondConstructorArgument.MappingKind)
                .IsEqualTo(AttributeDataViewModel.AttributeArgumentMappingKind.ParameterDefaultValue);

            await Assert.That(firstConstructorArgument.Value)
                .HasProperty(s => s.Kind, TypedConstantKind.Primitive)
                .And.HasProperty(s => s.Value, 3)
                .And.HasProperty(s => s.Type!.SpecialType, SpecialType.System_Int32);
            await Assert.That(secondConstructorArgument.Value)
                .HasProperty(s => s.Kind, TypedConstantKind.Primitive)
                .And.HasProperty(s => s.Value, 1)
                .And.HasProperty(s => s.Type!.SpecialType, SpecialType.System_Int32);

            await Assert.That(model.NamedArguments).IsEmpty();
        }
    }

    [Test]
    public async Task OneDefaultOneUnorderedDefaultConstructorOneNamedArguments(CancellationToken cancellationToken)
    {
        //lang=C#
        const string testSource =
            """
            using System;

            namespace XmlDocFormat.AttributeDataViewModelTests;

            public sealed class EmptyAttribute(int a = 1, int b = 2) : Attribute
            {
                public string? Preamble { get; set; }
            }

            [Empty(b: 3, Preamble = "Something")]
            public sealed class User;
            """;

        await VerifyCompilation(testSource, Verify, cancellationToken);

        static async Task Verify(
            Compilation compilation,
            CancellationToken cancellationToken)
        {
            var userClassSymbol = DeclaredSymbolOnNode(
                compilation, "class User", cancellationToken)!;
            var model = SingleAttributeDataViewModel(
                userClassSymbol, cancellationToken);

            await Assert.That(model).IsNotNull();

            var constructor = model.AttributeData.AttributeConstructor;
            await Assert.That(constructor).IsNotNull();
            var firstConstructorParameter = constructor.Parameters[0];
            var secondConstructorParameter = constructor.Parameters[1];

            var attributeClass = model.AttributeData.AttributeClass;
            await Assert.That(attributeClass).IsNotNull();
            var preambleProperty = attributeClass.GetMembers<IPropertySymbol>().Single();

            await Assert.That(model.ConstructorArguments).Count().IsEqualTo(2);
            var firstConstructorArgument = model.ConstructorArguments[0];
            var secondConstructorArgument = model.ConstructorArguments[1];
            await Assert.That(firstConstructorArgument.Name).IsEqualTo(secondConstructorParameter.Name);
            await Assert.That(secondConstructorArgument.Name).IsEqualTo(firstConstructorParameter.Name);

            await Assert.That(firstConstructorArgument.MappingKind)
                .IsEqualTo(AttributeDataViewModel.AttributeArgumentMappingKind.Parameter);
            await Assert.That(secondConstructorArgument.MappingKind)
                .IsEqualTo(AttributeDataViewModel.AttributeArgumentMappingKind.ParameterDefaultValue);

            await Assert.That(firstConstructorArgument.Value)
                .HasProperty(s => s.Kind, TypedConstantKind.Primitive)
                .And.HasProperty(s => s.Value, 3)
                .And.HasProperty(s => s.Type!.SpecialType, SpecialType.System_Int32);
            await Assert.That(secondConstructorArgument.Value)
                .HasProperty(s => s.Kind, TypedConstantKind.Primitive)
                .And.HasProperty(s => s.Value, 1)
                .And.HasProperty(s => s.Type!.SpecialType, SpecialType.System_Int32);

            var singleNamedArgument = await Assert.That(model.NamedArguments).HasSingleItem();
            await Assert.That(singleNamedArgument.Name).IsEqualTo(preambleProperty.Name);
            await Assert.That(singleNamedArgument.MappingKind)
                .IsEqualTo(AttributeDataViewModel.AttributeArgumentMappingKind.Named);
            await Assert.That(singleNamedArgument.Value)
                .HasProperty(s => s.Kind, TypedConstantKind.Primitive)
                .And.HasProperty(s => s.Value, "Something")
                .And.HasProperty(s => s.Type!.SpecialType, SpecialType.System_String);
            var attributeArgument = await Assert.That(singleNamedArgument.ArgumentSyntax)
                .IsTypeOf<AttributeArgumentSyntax>();
            await Assert.That(attributeArgument?.NameEquals).IsNotNull();
        }
    }

    [Test]
    public async Task UnorderedNamedConstructorArgumentsWithDefaultValue(CancellationToken cancellationToken)
    {
        //lang=C#
        const string testSource =
            """
            using System;

            namespace XmlDocFormat.AttributeDataViewModelTests;

            public sealed class EmptyAttribute(int a, int b, int c = 1) : Attribute;

            [Empty(b: 1, c: 4, a: 2)]
            public sealed class User;
            """;

        await VerifyCompilation(testSource, Verify, cancellationToken);

        static async Task Verify(
            Compilation compilation,
            CancellationToken cancellationToken)
        {
            var userClassSymbol = DeclaredSymbolOnNode(
                compilation, "class User", cancellationToken)!;
            var model = SingleAttributeDataViewModel(
                userClassSymbol, cancellationToken);

            await Assert.That(model).IsNotNull();

            var constructor = model.AttributeData.AttributeConstructor;
            await Assert.That(constructor).IsNotNull();
            var firstConstructorParameter = constructor.Parameters[0];
            var secondConstructorParameter = constructor.Parameters[1];
            var thirdConstructorParameter = constructor.Parameters[2];

            await Assert.That(model.ConstructorArguments).Count().IsEqualTo(3);
            var firstConstructorArgument = model.ConstructorArguments[0];
            var secondConstructorArgument = model.ConstructorArguments[1];
            var thirdConstructorArgument = model.ConstructorArguments[2];
            await Assert.That(firstConstructorArgument.Name).IsEqualTo(secondConstructorParameter.Name);
            await Assert.That(secondConstructorArgument.Name).IsEqualTo(thirdConstructorParameter.Name);
            await Assert.That(thirdConstructorArgument.Name).IsEqualTo(firstConstructorParameter.Name);

            await Assert.That(firstConstructorArgument.MappingKind)
                .IsEqualTo(AttributeDataViewModel.AttributeArgumentMappingKind.Parameter);
            await Assert.That(secondConstructorArgument.MappingKind)
                .IsEqualTo(AttributeDataViewModel.AttributeArgumentMappingKind.Parameter);
            await Assert.That(thirdConstructorArgument.MappingKind)
                .IsEqualTo(AttributeDataViewModel.AttributeArgumentMappingKind.Parameter);

            await Assert.That(firstConstructorArgument.Value)
                .HasProperty(s => s.Kind, TypedConstantKind.Primitive)
                .And.HasProperty(s => s.Value, 1)
                .And.HasProperty(s => s.Type!.SpecialType, SpecialType.System_Int32);
            await Assert.That(secondConstructorArgument.Value)
                .HasProperty(s => s.Kind, TypedConstantKind.Primitive)
                .And.HasProperty(s => s.Value, 4)
                .And.HasProperty(s => s.Type!.SpecialType, SpecialType.System_Int32);
            await Assert.That(thirdConstructorArgument.Value)
                .HasProperty(s => s.Kind, TypedConstantKind.Primitive)
                .And.HasProperty(s => s.Value, 2)
                .And.HasProperty(s => s.Type!.SpecialType, SpecialType.System_Int32);

            await Assert.That(model.NamedArguments).IsEmpty();
        }
    }

    [Test]
    public async Task OneParamsConstructorArgument(CancellationToken cancellationToken)
    {
        //lang=C#
        const string testSource =
            """
            using System;

            namespace XmlDocFormat.AttributeDataViewModelTests;

            public sealed class EmptyAttribute(params int[] a) : Attribute;

            [Empty(1, 2, 3)]
            public sealed class User;
            """;

        await VerifyCompilation(testSource, Verify, cancellationToken);

        static async Task Verify(
            Compilation compilation,
            CancellationToken cancellationToken)
        {
            var userClassSymbol = DeclaredSymbolOnNode(
                compilation, "class User", cancellationToken)!;
            var model = SingleAttributeDataViewModel(
                userClassSymbol, cancellationToken);

            await Assert.That(model).IsNotNull();
            var constructor = model.AttributeData.AttributeConstructor;
            await Assert.That(constructor).IsNotNull();
            var constructorParameter = constructor.Parameters.Single();

            await Assert.That(model.NamedArguments).IsEmpty();

            var singleArgument = await Assert.That(model.ConstructorArguments).HasSingleItem();
            await Assert.That(singleArgument.Name).IsEqualTo(constructorParameter.Name);
            await Assert.That(singleArgument.MappingKind)
                .IsEqualTo(AttributeDataViewModel.AttributeArgumentMappingKind.ParameterParamsExpansion);
            await Assert.That(singleArgument.Value)
                .HasProperty(s => s.Kind, TypedConstantKind.Array)
                .And.HasProperty(s => s.Values.Length, 3)
                .And.HasProperty(s => s.Type!.TypeKind, TypeKind.Array);
        }
    }

    [Test]
    public async Task OneNormalOneParamsConstructorOneNamedArgument(CancellationToken cancellationToken)
    {
        //lang=C#
        const string testSource =
            """
            using System;

            namespace XmlDocFormat.AttributeDataViewModelTests;

            public sealed class EmptyAttribute(int a, params int[] p) : Attribute
            {
                public string? Preamble { get; set; }
            }

            [Empty(1, 2, 3, Preamble = "Something")]
            public sealed class User;
            """;

        await VerifyCompilation(testSource, Verify, cancellationToken);

        static async Task Verify(
            Compilation compilation,
            CancellationToken cancellationToken)
        {
            var userClassSymbol = DeclaredSymbolOnNode(
                compilation, "class User", cancellationToken)!;
            var model = SingleAttributeDataViewModel(
                userClassSymbol, cancellationToken);

            await Assert.That(model).IsNotNull();
            var constructor = model.AttributeData.AttributeConstructor;
            await Assert.That(constructor).IsNotNull();
            var firstConstructorParameter = constructor.Parameters[0];
            var secondConstructorParameter = constructor.Parameters[1];

            var attributeClass = model.AttributeData.AttributeClass;
            await Assert.That(attributeClass).IsNotNull();
            var preambleProperty = attributeClass.GetMembers<IPropertySymbol>().Single();

            var singleNamedArgument = await Assert.That(model.NamedArguments).HasSingleItem();
            await Assert.That(singleNamedArgument.Name).IsEqualTo(preambleProperty.Name);
            await Assert.That(singleNamedArgument.MappingKind)
                .IsEqualTo(AttributeDataViewModel.AttributeArgumentMappingKind.Named);
            await Assert.That(singleNamedArgument.Value)
                .HasProperty(s => s.Kind, TypedConstantKind.Primitive)
                .And.HasProperty(s => s.Value, "Something")
                .And.HasProperty(s => s.Type!.SpecialType, SpecialType.System_String);
            var attributeArgument = await Assert.That(singleNamedArgument.ArgumentSyntax)
                .IsTypeOf<AttributeArgumentSyntax>();
            await Assert.That(attributeArgument?.NameEquals).IsNotNull();

            await Assert.That(model.ConstructorArguments).Count().IsEqualTo(2);
            var firstConstructorArgument = model.ConstructorArguments[0];
            var secondConstructorArgument = model.ConstructorArguments[1];
            await Assert.That(firstConstructorArgument.Name).IsEqualTo(firstConstructorParameter.Name);
            await Assert.That(secondConstructorArgument.Name).IsEqualTo(secondConstructorParameter.Name);

            await Assert.That(firstConstructorArgument.MappingKind)
                .IsEqualTo(AttributeDataViewModel.AttributeArgumentMappingKind.Parameter);
            await Assert.That(firstConstructorArgument.Value)
                .HasProperty(s => s.Kind, TypedConstantKind.Primitive)
                .And.HasProperty(s => s.Value, 1)
                .And.HasProperty(s => s.Type!.SpecialType, SpecialType.System_Int32);

            await Assert.That(secondConstructorArgument.MappingKind)
                .IsEqualTo(AttributeDataViewModel.AttributeArgumentMappingKind.ParameterParamsExpansion);
            await Assert.That(secondConstructorArgument.Value)
                .HasProperty(s => s.Kind, TypedConstantKind.Array)
                .And.HasProperty(s => s.Values.Length, 2)
                .And.HasProperty(s => s.Type!.TypeKind, TypeKind.Array);

            await Assert.That(secondConstructorArgument.ArgumentSyntax).IsNull();
            await Assert.That(secondConstructorArgument.ArgumentSyntaxList).Count().IsEqualTo(3);
            await Assert.That(secondConstructorArgument.ArgumentSyntaxList.ToFullString()).EqualTo("2, 3");
        }
    }

    private static AttributeDataViewModel? SingleAttributeDataViewModel(
        ISymbol symbol,
        CancellationToken cancellationToken)
    {
        var attribute = symbol.GetAttributes().FirstOrDefault();
        return attribute.SelectOrNull(
            a => AttributeDataViewModel.Create(a, cancellationToken));
    }

    private static ISymbol? DeclaredSymbolOnNode(
        Compilation compilation,
        string substring,
        CancellationToken cancellationToken)
    {
        var tree = compilation.SyntaxTrees.First();
        var declaration = tree.FindNodeContaining(
            substring, cancellationToken: cancellationToken)!;
        var semanticModel = compilation.GetSemanticModel(tree);
        return semanticModel.GetDeclaredSymbol(declaration, cancellationToken);
    }
}
