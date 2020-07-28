﻿#nullable enable
namespace ScTools.ScriptLang.Ast
{
    using System.Linq;

    using Antlr4.Runtime;
    using Antlr4.Runtime.Misc;

    using ScTools.ScriptLang.Grammar;

    public sealed class AstBuilder : ScLangBaseVisitor<Node>
    {
        private sealed class PlaceholderStatement : Statement
        {
            public PlaceholderStatement(SourceLocation location) : base(location) { }
            public override string ToString() => "PlaceholderStmt";
        }

        private sealed class PlaceholderExpression : Expression
        {
            public PlaceholderExpression(SourceLocation location) : base(location) { }
            public override string ToString() => "PlaceholderExpr";
        }

        private static SourceLocation Source(ParserRuleContext context) => new SourceLocation(context.Start.Line, context.Start.Column + 1);
        private static Node PlaceholderStmt(ScLangParser.StatementContext context) => new PlaceholderStatement(Source(context));
        private static Node PlaceholderExpr(ScLangParser.ExpressionContext context) => new PlaceholderExpression(Source(context));

        public override Node VisitScript([NotNull] ScLangParser.ScriptContext context)
            => new Root(context.topLevelStatement().Select(stmt => stmt.Accept(this)).Cast<TopLevelStatement>(), Source(context));

        #region Top Level Statements
        public override Node VisitScriptNameStatement([NotNull] ScLangParser.ScriptNameStatementContext context)
            => new ScriptNameStatement((Identifier)context.identifier().Accept(this), Source(context));

        public override Node VisitProcedureStatement([NotNull] ScLangParser.ProcedureStatementContext context)
            => new ProcedureStatement((Identifier)context.procedure().identifier().Accept(this),
                                      (StatementBlock)context.procedure().statementBlock().Accept(this),
                                      Source(context));

        public override Node VisitStructStatement([NotNull] ScLangParser.StructStatementContext context)
            => new StructStatement((Identifier)context.@struct().identifier().Accept(this), Source(context));

        public override Node VisitStaticFieldStatement([NotNull] ScLangParser.StaticFieldStatementContext context)
            => new StaticFieldStatement((Identifier)context.variableDeclaration().variable().identifier().Accept(this),
                                        Source(context));
        #endregion Top Level Statements

        #region Statements
        public override Node VisitVariableDeclarationStatement([NotNull] ScLangParser.VariableDeclarationStatementContext context)
            => new VariableDeclarationStatement((Variable)context.variableDeclaration().variable().Accept(this),
                                                (Expression?)context.variableDeclaration().expression()?.Accept(this),
                                                Source(context));

        public override Node VisitAssignmentStatement([NotNull] ScLangParser.AssignmentStatementContext context)
            => new AssignmentStatement((Expression)context.left.Accept(this),
                                       (Expression)context.right.Accept(this),
                                       Source(context));

        public override Node VisitIfStatement([NotNull] ScLangParser.IfStatementContext context)
            => new IfStatement((Expression)context.condition.Accept(this),
                               (StatementBlock)context.statementBlock().Accept(this),
                               Source(context));

        public override Node VisitWhileStatement([NotNull] ScLangParser.WhileStatementContext context)
            => new WhileStatement((Expression)context.condition.Accept(this),
                                  (StatementBlock)context.statementBlock().Accept(this),
                                  Source(context));

        public override Node VisitCallStatement([NotNull] ScLangParser.CallStatementContext context)
            => new CallStatement((ProcedureCall)context.procedureCall().Accept(this), Source(context));
        #endregion Statements

        #region Expressions
        public override Node VisitNotExpression([NotNull] ScLangParser.NotExpressionContext context)
            => PlaceholderExpr(context);

        public override Node VisitBinaryExpression([NotNull] ScLangParser.BinaryExpressionContext context)
            => PlaceholderExpr(context);

        public override Node VisitAggregateExpression([NotNull] ScLangParser.AggregateExpressionContext context)
            => PlaceholderExpr(context);

        public override Node VisitIdentifierExpression([NotNull] ScLangParser.IdentifierExpressionContext context)
            => PlaceholderExpr(context);

        public override Node VisitMemberAccessExpression([NotNull] ScLangParser.MemberAccessExpressionContext context)
            => PlaceholderExpr(context);

        public override Node VisitArrayAccessExpression([NotNull] ScLangParser.ArrayAccessExpressionContext context)
            => PlaceholderExpr(context);

        public override Node VisitCallExpression([NotNull] ScLangParser.CallExpressionContext context)
            => PlaceholderExpr(context);

        public override Node VisitLiteralExpression([NotNull] ScLangParser.LiteralExpressionContext context)
            => PlaceholderExpr(context);
        #endregion Expressions

        #region Misc
        public override Node VisitStatementBlock([NotNull] ScLangParser.StatementBlockContext context)
            => new StatementBlock(context.statement().Select(stmt => stmt.Accept(this)).Cast<Statement>(), Source(context));

        public override Node VisitProcedureCall([NotNull] ScLangParser.ProcedureCallContext context)
            => new ProcedureCall((Identifier)context.identifier().Accept(this),
                                 context.expression().Select(expr => expr.Accept(this)).Cast<Expression>(),
                                 Source(context));

        public override Node VisitVariable([NotNull] ScLangParser.VariableContext context)
            => new Variable((Type)context.type().Accept(this),
                            (Identifier)context.identifier().Accept(this),
                            (ArrayIndexer?)context.arrayIndexer()?.Accept(this),
                            Source(context));

        public override Node VisitType([NotNull] ScLangParser.TypeContext context)
            => new Type((Identifier)context.identifier().Accept(this), Source(context));

        public override Node VisitIdentifier([NotNull] ScLangParser.IdentifierContext context)
            => new Identifier(context.GetText(), Source(context));
        #endregion Misc
    }
}
