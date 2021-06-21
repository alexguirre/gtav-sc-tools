﻿namespace ScTools.ScriptLang.Ast.Types
{
    using System;

    using ScTools.ScriptLang.Ast.Errors;
    using ScTools.ScriptLang.Ast.Expressions;

    public interface IType : INode
    {
        int SizeOf { get; }

        /// <summary>
        /// Gets the <see cref="IType"/> this type should be treated as when used as a value.
        /// This is the same as this type except for <see cref="RefType"/>s where it returns its <see cref="RefType.PointeeType"/>.
        /// </summary>
        IType ByValue { get; }

        /// <summary>
        /// Gets whether this and <paramref name="other"/> represent the same type.
        /// </summary>
        bool Equivalent(IType other);

        /// <summary>
        /// Gets whether a <see cref="RefType"/> of this type can be binded to a lvalue of type <paramref name="other"/>.
        /// </summary>
        bool CanBindRefTo(IType other);

        /// <summary>
        /// Gets whether the type <paramref name="rhs"/> can be assigned to this type.
        /// </summary>
        bool CanAssign(IType rhs, bool rhsIsLValue);

        /// <summary>
        /// Gets whether the type <paramref name="rhs"/> can be assigned to this type in an initializer or when passed as parameter.
        /// </summary>
        bool CanAssignInit(IType rhs, bool rhsIsLValue);

        // Semantic Checks

        /// <summary>
        /// Checks if this type supports the binary operation <paramref name="op"/> with the type <paramref name="rhs"/> and returns the resulting type.
        /// </summary>
        IType BinaryOperation(BinaryOperator op, IType rhs, SourceRange source, DiagnosticsReport diagnostics);

        /// <summary>
        /// Checks if this type supports the unary operation <paramref name="op"/> and returns the resulting type.
        /// </summary>
        IType UnaryOperation(UnaryOperator op, SourceRange source, DiagnosticsReport diagnostics);

        /// <summary>
        /// Checks if this type has the field <paramref name="fieldName"/> and returns its type and whether it is an lvalue.
        /// </summary>
        (IType Type, bool LValue) FieldAccess(string fieldName, SourceRange source, DiagnosticsReport diagnostics);

        /// <summary>
        /// Checks if this type supports indexing with <paramref name="index"/> and returns the resulting item type.
        /// </summary>
        IType Indexing(IType index, SourceRange source, DiagnosticsReport diagnostics);

        /// <summary>
        /// Checks if this type supports invocation with the specified <paramref name="args"/> and returns the returned type.
        /// </summary>
        IType Invocation((IType Type, bool IsLValue, SourceRange Source)[] args, SourceRange source, DiagnosticsReport diagnostics);

        /// <summary>
        /// Checks if the type <paramref name="rhs"/> can be assigned to this type.
        /// </summary>
        void Assign(IType rhs, bool rhsIsLValue, SourceRange source, DiagnosticsReport diagnostics);

        /// <summary>
        /// Checks if the type <paramref name="rhs"/> can be assigned to this type in an initializer or when passed as parameter.
        /// </summary>
        void AssignInit(IType rhs, bool rhsIsLValue, SourceRange source, DiagnosticsReport diagnostics);
    }

    public interface IArrayType : IType
    {
        IType ItemType { get; set; }
    }

    public abstract class BaseType: BaseNode, IType
    {
        public abstract int SizeOf { get; }
        public virtual IType ByValue => this;

        public BaseType(SourceRange source) : base(source) {}

        public override string ToString() => TypePrinter.ToString(this, null);

        public abstract bool Equivalent(IType other);
        public virtual bool CanBindRefTo(IType other) => Equivalent(other);
        public virtual bool CanAssign(IType rhs, bool rhsIsLValue) => false;
        public virtual bool CanAssignInit(IType rhs, bool rhsIsLValue) => CanAssign(rhs, rhsIsLValue);

        public virtual IType BinaryOperation(BinaryOperator op, IType rhs, SourceRange source, DiagnosticsReport diagnostics)
            => new ErrorType(source, diagnostics, $"Binary operator '{op.ToToken()}' is not supported with operands of type '{this}' and '{rhs}'");

        public virtual IType UnaryOperation(UnaryOperator op, SourceRange source, DiagnosticsReport diagnostics)
            => new ErrorType(source, diagnostics, $"Unary operator '{op.ToToken()}' is not supported by type '{this}'");

        public virtual (IType Type, bool LValue) FieldAccess(string fieldName, SourceRange source, DiagnosticsReport diagnostics)
            => (new ErrorType(source, diagnostics, $"Field access is not supported by type '{this}'"), false);

        public virtual IType Indexing(IType index, SourceRange source, DiagnosticsReport diagnostics)
            => new ErrorType(source, diagnostics, $"Indexing is not supported by type '{this}'");

        public virtual IType Invocation((IType Type, bool IsLValue, SourceRange Source)[] args, SourceRange source, DiagnosticsReport diagnostics)
            => new ErrorType(source, diagnostics, $"Invocation is not supported by type '{this}'");

        public virtual void Assign(IType rhs, bool rhsIsLValue, SourceRange source, DiagnosticsReport diagnostics)
        {
            if (CanAssign(rhs, rhsIsLValue))
            {
                return;
            }

            diagnostics.AddError($"Cannot assign type '{rhs}' to '{this}'", source);
        }

        public virtual void AssignInit(IType rhs, bool rhsIsLValue, SourceRange source, DiagnosticsReport diagnostics)
        {
            if (CanAssignInit(rhs, rhsIsLValue))
            {
                return;
            }

            diagnostics.AddError($"Cannot assign type '{rhs}' to '{this}'", source);
        }
    }

    public abstract class BaseArrayType : BaseType, IArrayType
    {
        public IType ItemType { get; set; }

        public BaseArrayType(SourceRange source, IType itemType) : base(source)
            => ItemType = itemType;

        public override (IType Type, bool LValue) FieldAccess(string fieldName, SourceRange source, DiagnosticsReport diagnostics)
        {
            if (Parser.CaseInsensitiveComparer.Equals(fieldName, "length"))
            {
                return (new IntType(source), false);
            }
            else
            {
                return (new ErrorType(source, diagnostics, $"Unknown field '{fieldName}'"), false);
            }
        }

        public override IType Indexing(IType index, SourceRange source, DiagnosticsReport diagnostics)
        {
            var expectedIndexTy = new IntType(source);
            if (!expectedIndexTy.CanAssign(index, rhsIsLValue: false))
            {
                return new ErrorType(source, diagnostics, $"Expected type '{expectedIndexTy}' as array index, found '{index}'");
            }

            return ItemType;
        }
    }
}
