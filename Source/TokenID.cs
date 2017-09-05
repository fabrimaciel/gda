﻿/* 
 * GDA - Generics Data Access, is framework to object-relational mapping 
 * (a programming technique for converting data between incompatible 
 * type systems in databases and Object-oriented programming languages) using c#.
 * 
 * Copyright (C) 2010  <http://www.colosoft.com.br/gda> - support@colosoft.com.br
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace GDA.Sql.InterpreterExpression.Enums
{
	enum TokenID : short
	{
		kAbsolute,
		kAction,
		kAdd,
		kAll,
		kAllocate,
		kAlter,
		kAnd,
		kAny,
		kAre,
		kAs,
		kAsc,
		kAssertion,
		kAt,
		kAuthorization,
		kAvg,
		kBegin,
		kBetween,
		kBit,
		kBit_Length,
		kBoth,
		kBy,
		kCascade,
		kCascaded,
		kCase,
		kCast,
		kCatalog,
		kChar,
		kChar_Length,
		kCharacter,
		kCharacter_Length,
		kCheck,
		kClose,
		kCoalesce,
		kCollate,
		kCollation,
		kColumn,
		kCommit,
		kConnect,
		kConnection,
		kConstraint,
		kConstraints,
		kContinue,
		kConvert,
		kCorresponding,
		kCount,
		kCreate,
		kCross,
		kCurrent,
		kCurrent_Date,
		kCurrent_Time,
		kCurrent_Timestamp,
		kCurrent_User,
		kCursor,
		kDate,
		kDay,
		kDeallocate,
		kDec,
		kDecimal,
		kDeclare,
		kDefault,
		kDeferrable,
		kDeferred,
		kDelete,
		kDesc,
		kDescribe,
		kDescriptor,
		kDiagnostics,
		kDisconnect,
		kDistinct,
		kDomain,
		kDouble,
		kDrop,
		kElse,
		kEnd,
		kEnd_Exec,
		kEscape,
		kExcept,
		kException,
		kExec,
		kExecute,
		kExists,
		kExternal,
		kExtract,
		kFalse,
		kFetch,
		kFirst,
		kFloat,
		kFor,
		kForeign,
		kFortran,
		kFound,
		kFrom,
		kFull,
		kGet,
		kGlobal,
		kGo,
		kGoto,
		kGrant,
		kGroup,
		kHaving,
		kHour,
		kIdentity,
		kImmediate,
		kIn,
		kInclude,
		kIndex,
		kIndicator,
		kInitially,
		kInner,
		kInput,
		kInsensitive,
		kInsert,
		kInt,
		kInteger,
		kIntersect,
		kInterval,
		kInto,
		kIs,
		kIsNull,
		kIsolation,
		kJoin,
		kKey,
		kLanguage,
		kLast,
		kLeading,
		kLeft,
		kLevel,
		kLike,
		kLocal,
		kLower,
		kMatch,
		kMax,
		kMin,
		kMinute,
		kModule,
		kMonth,
		kNames,
		kNational,
		kNatural,
		kNChar,
		kNext,
		kNo,
		kNone,
		kNot,
		kNull,
		kNullIf,
		kNulls,
		kNumeric,
		kOctect_Length,
		kOf,
		kOn,
		kOnly,
		kOpen,
		kOption,
		kOr,
		kOrder,
		kOuter,
		kOutput,
		kOverlaps,
		kPad,
		kPartial,
		kPascal,
		kPercent,
		kPosition,
		kPrecision,
		kPrepare,
		kPreserve,
		kPrimary,
		kPrior,
		kPrivileges,
		kProcedure,
		kPublic,
		kRead,
		kReal,
		kReferences,
		kRelative,
		kRestrict,
		kRevoke,
		kRight,
		kRollback,
		kRows,
		kSchema,
		kScroll,
		kSecond,
		kSection,
		kSelect,
		kSession,
		kSession_User,
		kSet,
		kSize,
		kSmallint,
		kSome,
		kSpace,
		kSql,
		kSqlCa,
		kSqlCode,
		kSqlError,
		kSqlState,
		kSqlWarning,
		kSubstring,
		kSum,
		kSystem_User,
		kTable,
		kTemporary,
		kThen,
		kTime,
		kTimestamp,
		kTimezone_Hour,
		kTimezone_Minute,
		kTo,
		kTop,
		kTrailing,
		kTransaction,
		kTranslate,
		kTranslation,
		kTrim,
		kTrue,
		kUnion,
		kUnique,
		kUnknown,
		kUpdate,
		kUpper,
		kUsage,
		kUser,
		kUsing,
		kValue,
		kValues,
		kVarchar,
		kVarying,
		kView,
		kWhen,
		kWhenever,
		kWhere,
		kWith,
		kWork,
		kWrite,
		kYear,
		kZone,
		Whitespace,
		Newline,
		SingleComment,
		BMultiComment,
		EMultiComment,
		DocComment,
		HexLiteral,
		IntLiteral,
		StringLiteral,
		DecimalLiteral,
		RealLiteral,
		Dot,
		Quote,
		Hash,
		Dollar,
		Percent,
		BAnd,
		SQuote,
		Star,
		Plus,
		Comma,
		Minus,
		Slash,
		BSQuote,
		Colon,
		Semi,
		Less,
		Equal,
		Greater,
		Question,
		And,
		Or,
		PlusPlus,
		MinusMinus,
		MinusGreater,
		EqualEqual,
		NotEqual,
		LessEqual,
		GreaterEqual,
		PlusEqual,
		LParen,
		RParen,
		LBracket,
		RBracket,
		LCurly,
		RCurly,
		InvalidTab,
		Identifier,
		End,
		InvalidExpression
	}
}
