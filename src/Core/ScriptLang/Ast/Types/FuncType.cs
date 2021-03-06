﻿namespace ScTools.ScriptLang.Ast.Types
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using ScTools.ScriptAssembly;
    using ScTools.ScriptLang.Ast.Declarations;
    using ScTools.ScriptLang.Ast.Errors;
    using ScTools.ScriptLang.Ast.Expressions;
    using ScTools.ScriptLang.BuiltIns;
    using ScTools.ScriptLang.CodeGen;

    public sealed class FuncType : BaseType
    {
        public override int SizeOf => 1;
        public FuncProtoDeclaration Declaration { get; set; }

        public FuncType(SourceRange source, FuncProtoDeclaration declaration) : base(source)
            => Declaration = declaration;

        public override TReturn Accept<TReturn, TParam>(IVisitor<TReturn, TParam> visitor, TParam param)
            => visitor.Visit(this, param);

        public override bool Equivalent(IType other)
            => other is FuncType otherFunc &&
               Declaration.Kind == otherFunc.Declaration.Kind &&
               Declaration.Parameters.Count == otherFunc.Declaration.Parameters.Count &&
               Declaration.ReturnType.Equivalent(otherFunc.Declaration.ReturnType) &&
               Declaration.Parameters.Zip(otherFunc.Declaration.Parameters).All(p => p.First.Type.Equivalent(p.Second.Type));

        public override bool CanAssign(IType rhs, bool rhsIsLValue) => rhs is NullType or ErrorType || Equivalent(rhs);

        public override IType BinaryOperation(BinaryOperator op, IType rhs, SourceRange source, DiagnosticsReport diagnostics)
        {
            if (op is BinaryOperator.Equals or BinaryOperator.NotEquals)
            {
                if (rhs is NullType || Equivalent(rhs))
                {
                    return new BoolType(source);
                }
            }

            return base.BinaryOperation(op, rhs, source, diagnostics);
        }

        public override (IType ReturnType, bool IsConstant) Invocation(IExpression[] args, SourceRange source, DiagnosticsReport diagnostics)
        {
            if (Declaration.Kind is FuncKind.Script)
            {
                return (new ErrorType(source, diagnostics, "Cannot invoke SCRIPT"), false);
            }

            var parameters = Declaration.Parameters;
            if (args.Length != parameters.Count)
            {
                diagnostics.AddError($"Expected {parameters.Count} arguments, found {args.Length}", source);
            }

            var n = Math.Min(args.Length, parameters.Count);
            for (int i = 0; i < n; i++)
            {
                var param = parameters[i];
                var arg = args[i];
                if (param.IsReference)
                {
                    if (!arg.IsLValue)
                    {
                        diagnostics.AddError($"Argument {i + 1}: cannot bind parameter '{TypePrinter.ToString(param.Type, param.Name, param.IsReference)}' to non-lvalue", arg.Source);
                    }
                    else if (!param.Type.CanBindRefTo(arg.Type))
                    {
                        diagnostics.AddError($"Argument {i + 1}: cannot bind parameter '{TypePrinter.ToString(param.Type, param.Name, param.IsReference)}' to reference of type '{arg.Type}'", arg.Source);
                    }
                }
                else
                {
                    // TODO: this check here to disallow passing INT/STRING/TEXT_LABELs of different size to TEXT_LABELs
                    //       (which TextLabelType.CanAssign allows) is a bit of a hack.
                    //       Supporting this would require creating a temporary variable because the opcodes used for these
                    //       conversions need an address to write to, but currently temporary variables are not supported
                    //       anywhere in the compiler

                    if ((param.Type is TextLabelType && !param.Type.Equivalent(arg.Type!)) ||
                        !param.Type.CanAssign(arg.Type!, arg.IsLValue))
                    {
                        diagnostics.AddError($"Argument {i + 1}: cannot pass '{arg.Type}' as parameter '{TypePrinter.ToString(param.Type, param.Name, param.IsReference)}'", arg.Source);
                    }
                }
            }

            return (Declaration.ReturnType, false);
        }

        public override void CGBinaryOperation(CodeGenerator cg, BinaryExpression expr)
        {
            cg.EmitValue(expr.LHS);
            cg.EmitValue(expr.RHS);
            switch (expr.Operator)
            {
                case BinaryOperator.Equals: cg.Emit(Opcode.IEQ); break;
                case BinaryOperator.NotEquals: cg.Emit(Opcode.INE); break;

                default: throw new NotImplementedException();
            }
        }

        public override void CGInvocation(CodeGenerator cg, InvocationExpression expr)
        {
            if (expr.Callee is DeclarationRefExpression { Declaration: FuncDeclaration func })
            {
                switch (Declaration.Kind)
                {
                    case FuncKind.UserDefined:
                        EmitArgs(cg, func.Prototype.Parameters, expr.Arguments);
                        cg.EmitCall(func.Name);
                        break;

                    case FuncKind.Native:
                        EmitArgs(cg, func.Prototype.Parameters, expr.Arguments);
                        cg.EmitNativeCall(Declaration.ParametersSize, Declaration.ReturnType.SizeOf, func.Name);
                        break;

                    case FuncKind.Script: throw new InvalidOperationException("Cannot invoke SCRIPT");

                    default: throw new NotImplementedException();
                }
            }
            else
            {
                var parameters = ((FuncType)expr.Callee.Type!).Declaration.Parameters;
                EmitArgs(cg, parameters, expr.Arguments);
                cg.EmitValue(expr.Callee);
                cg.Emit(Opcode.CALLINDIRECT);
            }

            static void EmitArgs(CodeGenerator cg, IList<VarDeclaration> parameters, IList<IExpression> arguments)
            {
                foreach (var (p, a) in parameters.Zip(arguments))
                {
                    EmitArg(cg, p, a);
                }
            }

            static void EmitArg(CodeGenerator cg, VarDeclaration param, IExpression arg)
            {
                // TODO: this TEXT_LABEL -> STRING conversion check here is a bit of a hack

                if (param.IsReference || (param.Type is StringType && arg.Type is TextLabelType))
                {
                    Debug.Assert(arg.IsLValue);
                    cg.EmitAddress(arg);
                }
                else
                {
                    cg.EmitValue(arg);
                }
            }
        }
    }
}
