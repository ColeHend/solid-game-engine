declare namespace System {
	/** Base class for all exceptions in .NET. */
	class Exception extends Error {
			readonly Message: string;
			readonly StackTrace: string;
			InnerException?: Exception;

			constructor(message?: string, innerException?: Exception);
			ToString(): string;
	}

	/** Represents an instant in time, typically expressed as a date and time of day. */
	class DateTime implements IComparable<DateTime>, IEquatable<DateTime> {
			static Now: DateTime;
			static UtcNow: DateTime;
			static Today: DateTime;

			readonly Date: DateTime;
			readonly Day: number;
			readonly Month: number;
			readonly Year: number;
			readonly Hour: number;
			readonly Minute: number;
			readonly Second: number;
			readonly Millisecond: number;
			readonly Kind: DateTimeKind;

			constructor(
					year: number,
					month: number,
					day: number,
					hour?: number,
					minute?: number,
					second?: number,
					millisecond?: number,
					kind?: DateTimeKind
			);

			Add(value: TimeSpan): DateTime;
			Subtract(value: TimeSpan): DateTime;
			Subtract(value: DateTime): TimeSpan;
			ToLocalTime(): DateTime;
			ToUniversalTime(): DateTime;
			ToString(format?: string): string;
			// Additional methods...
	}

	/** Specifies whether a DateTime object represents local time, UTC, or is unspecified. */
	enum DateTimeKind {
			Unspecified,
			Utc,
			Local,
	}

	/** Represents a time interval. */
	class TimeSpan implements IComparable<TimeSpan>, IEquatable<TimeSpan> {
			static Zero: TimeSpan;

			readonly Days: number;
			readonly Hours: number;
			readonly Minutes: number;
			readonly Seconds: number;
			readonly Milliseconds: number;
			readonly Ticks: number;
			readonly TotalDays: number;
			readonly TotalHours: number;
			readonly TotalMinutes: number;
			readonly TotalSeconds: number;
			readonly TotalMilliseconds: number;

			constructor(
					days: number,
					hours: number,
					minutes: number,
					seconds?: number,
					milliseconds?: number
			);

			Add(ts: TimeSpan): TimeSpan;
			Subtract(ts: TimeSpan): TimeSpan;
			Duration(): TimeSpan;
			Negate(): TimeSpan;
			ToString(): string;
			// Additional methods...
	}

	/** Represents a 32-bit signed integer. */
	class Int32 implements IComparable<Int32>, IEquatable<Int32> {
			static MaxValue: number;
			static MinValue: number;

			constructor(value: number);

			CompareTo(value: Int32): number;
			Equals(value: Int32): boolean;
			ToString(): string;
			// Additional methods...
	}

	/** Provides methods for manipulating arrays. */
	class Array {
			static Empty<T>(): T[];
			static Resize<T>(array: T[], newSize: number): void;
			static IndexOf<T>(array: T[], value: T, startIndex?: number): number;
			static LastIndexOf<T>(array: T[], value: T, startIndex?: number): number;
			// Additional static methods...
	}

	/** Represents text as a series of Unicode characters. */
	class String implements IComparable<String>, IEquatable<String>, Iterable<string> {
			static Empty: string;

			readonly Length: number;

			constructor(value?: string);

			CharAt(index: number): string;
			Concat(...strings: string[]): string;
			IndexOf(value: string, startIndex?: number): number;
			Substring(startIndex: number, length?: number): string;
			ToLower(): string;
			ToUpper(): string;
			Trim(): string;
			[Symbol.iterator](): Iterator<string>;
			// Additional methods and overloads...
	}

	/** Provides information about the environment. */
	class Environment {
			static readonly NewLine: string;
			static readonly TickCount: number;
			static Exit(exitCode: number): void;
			static GetEnvironmentVariable(variable: string): string | null;
			// Additional properties and methods...
	}

	/** Represents the base class for all tasks. */
	class Task implements Promise<void> {
			// Promise interface implementation
			then<TResult1 = void, TResult2 = never>(
					onfulfilled?: ((value: void) => TResult1 | PromiseLike<TResult1>) | undefined | null,
					onrejected?: ((reason: any) => TResult2 | PromiseLike<TResult2>) | undefined | null
			): Promise<TResult1 | TResult2>;
			catch<TResult = never>(
					onrejected?: ((reason: any) => TResult | PromiseLike<TResult>) | undefined | null
			): Promise<void | TResult>;
			finally(onfinally?: (() => void) | undefined | null): Promise<void>;

			// Task-specific members
			static Run(action: () => void): Task;
			// Additional methods...
	}

	/** Represents a task that returns a result. */
	class Task<TResult> implements Promise<TResult> {
			// Promise interface implementation
			then<TResult1 = TResult, TResult2 = never>(
					onfulfilled?: ((value: TResult) => TResult1 | PromiseLike<TResult1>) | undefined | null,
					onrejected?: ((reason: any) => TResult2 | PromiseLike<TResult2>) | undefined | null
			): Promise<TResult1 | TResult2>;
			catch<TResult = never>(
					onrejected?: ((reason: any) => TResult | PromiseLike<TResult>) | undefined | null
			): Promise<TResult | TResult>;
			finally(onfinally?: (() => void) | undefined | null): Promise<TResult>;

			// Task-specific members
			static Run<TResult>(func: () => TResult): Task<TResult>;
			Result: TResult;
			// Additional methods...
	}

	/** Represents a globally unique identifier (GUID). */
	class Guid implements IComparable<Guid>, IEquatable<Guid> {
			static Empty: Guid;

			constructor(guidString: string);

			ToString(): string;
			// Additional methods...
	}

	/** Provides random number generation. */
	class Random {
			constructor(seed?: number);

			Next(maxValue?: number): number;
			Next(minValue: number, maxValue: number): number;
			NextDouble(): number;
			// Additional methods...
	}

	/** Defines a generalized method that a value type or class implements to create a type-specific method for determining equality of instances. */
	interface IEquatable<T> {
			Equals(other: T): boolean;
	}

	/** Defines a generalized type-specific comparison method that a value type or class implements to order or sort its instances. */
	interface IComparable<T> {
			CompareTo(other: T): number;
	}

	/** Represents errors that occur during application execution. */
	class Error implements Exception {
			name: string;
			message: string;
			stack?: string | undefined;

			constructor(message?: string);

			// Implement Exception members
			readonly Message: string;
			readonly StackTrace: string;
			InnerException?: Exception;
			ToString(): string;
	}

	/** Provides a mechanism for releasing unmanaged resources. */
	interface IDisposable {
			Dispose(): void;
	}

	/** Encapsulates a method that has a single parameter and does not return a value. */
	interface Action<T> {
			(obj: T): void;
	}

	/** Encapsulates a method that has no parameters and does not return a value. */
	interface Action {
			(): void;
	}

	/** Encapsulates a method that has a single parameter and returns a value of the type specified by the TResult parameter. */
	interface Func<T, TResult> {
			(arg: T): TResult;
	}

	/** Encapsulates a method that has no parameters and returns a value of the type specified by the TResult parameter. */
	interface Func<TResult> {
			(): TResult;
	}

	/** Represents a method that handles events without event data. */
	interface EventHandler {
			(sender: any, e: EventArgs): void;
	}

	/** Represents the base class for classes that contain event data. */
	class EventArgs {
			static Empty: EventArgs;
	}

	/** Provides methods to create, copy, delete, move, and open files, and helps create FileStream objects. */
	namespace IO {
			class File {
					static Exists(path: string): boolean;
					static ReadAllText(path: string): string;
					static WriteAllText(path: string, contents: string): void;
					// Additional methods...
			}

			class Directory {
					static Exists(path: string): boolean;
					static GetFiles(path: string): string[];
					static CreateDirectory(path: string): void;
					// Additional methods...
			}

			class Path {
					static Combine(...paths: string[]): string;
					static GetFileName(path: string): string;
					static GetExtension(path: string): string;
					// Additional methods...
			}

			interface Stream extends IDisposable {
					readonly CanRead: boolean;
					readonly CanWrite: boolean;
					readonly CanSeek: boolean;
					readonly Length: number;
					Position: number;

					Read(buffer: Uint8Array, offset: number, count: number): number;
					Write(buffer: Uint8Array, offset: number, count: number): void;
					Seek(offset: number, origin: SeekOrigin): number;
					Flush(): void;
					// Additional methods...
			}

			enum SeekOrigin {
					Begin,
					Current,
					End,
			}
	}

	/** Provides methods for querying objects that implement IEnumerable<T>. */
	namespace Linq {
			class Enumerable {
					static Where<T>(source: Iterable<T>, predicate: (item: T) => boolean): Iterable<T>;
					static Select<TSource, TResult>(
							source: Iterable<TSource>,
							selector: (item: TSource) => TResult
					): Iterable<TResult>;
					static ToArray<T>(source: Iterable<T>): T[];
					// Additional methods...
			}
	}

	/** Provides basic mathematical functions. */
	class Math {
			static readonly E: number;
			static readonly PI: number;

			static Abs(value: number): number;
			static Acos(value: number): number;
			static Asin(value: number): number;
			static Atan(value: number): number;
			static Ceiling(value: number): number;
			static Cos(value: number): number;
			static Exp(value: number): number;
			static Floor(value: number): number;
			static Log(value: number, base?: number): number;
			static Max(value1: number, value2: number): number;
			static Min(value1: number, value2: number): number;
			static Pow(x: number, y: number): number;
			static Round(value: number, digits?: number): number;
			static Sin(value: number): number;
			static Sqrt(value: number): number;
			static Tan(value: number): number;
			// Additional methods...
	}

	/** Represents a key/value pair in a dictionary. */
	class KeyValuePair<TKey, TValue> {
			constructor(key: TKey, value: TValue);

			readonly Key: TKey;
			readonly Value: TValue;
	}

	namespace Collections {
			namespace Generic {
					/** Represents a collection of keys and values. */
					class Dictionary<TKey, TValue> implements Iterable<KeyValuePair<TKey, TValue>> {
							constructor();

							Add(key: TKey, value: TValue): void;
							Remove(key: TKey): boolean;
							TryGetValue(key: TKey, value: TValue): boolean;
							readonly Count: number;
							Keys: Iterable<TKey>;
							Values: Iterable<TValue>;
							[Symbol.iterator](): Iterator<KeyValuePair<TKey, TValue>>;
							// Additional methods...
					}

					/** Represents a list of objects that can be accessed by index. */
					class List<T> implements Iterable<T> {
							constructor(list: T[]);

							Add(item: T): void;
							Remove(item: T): boolean;
							readonly Count: number;
							[index: number]: T;
							[Symbol.iterator](): Iterator<T>;
							// Additional methods...
					}
			}
	}

	namespace Text {
			/** Represents a mutable string of characters. */
			class StringBuilder {
					constructor(value?: string);

					Append(value: string): StringBuilder;
					Remove(startIndex: number, length: number): StringBuilder;
					ToString(): string;
					// Additional methods...
			}

			/** Represents a character encoding. */
			class Encoding {
					static UTF8: Encoding;
					static ASCII: Encoding;
					// Additional static properties...

					GetBytes(s: string): Uint8Array;
					GetString(bytes: Uint8Array): string;
					// Additional methods...
			}
	}
}
