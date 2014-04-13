/*
 * Provides invocable hooks/event subscribing functionality natively in C#. Baremetal implementation, no type checking or run-time compilation required.
 * 
 * How to use:
 * -Create a new hook with an index and an identifier. The index will be an enumerated value.
 * -Associate the index or identifier with either a lambda function, static method or an instance's method
 * -Call on the index or identifier of the hook.
 * -All of the methods will now be called.
 * 
 * Mockup Examples (Different things you can do):
 * 	 Invocables.Hook.Create(Invocables.HOOKS.USED_ITEM, "USED_ITEM");
 *	 Invocables.Hook.Create(1, "HOOK_TEST");
 *   
 * 	 //You can reference a method in an object, parameters are passed during call (System.Object[] para)
 *   	Invocables.Hook.Add("HOOK_TEST", StaticObject.OnMethod);
 * 	 //Lambda functions	 
 *   	string msg1 = "Hook Call 1";	 
 *   	Invocables.Hook.Add(Ivokables.HOOKS.USED_ITEM, (msg2) => {UnityEngine.Debug.Log(msg1); UnityEngine.Debug.Log(msg2[0]);});
 * 	 //Access with 'para[index]', cast if necessary 'var value = (SomeObject)para[1]'
 *   	Invocables.Hook.Add(Ivokables.HOOKS.USED_ITEM, Instance.OnMethod);
 *
 *   //Call all methods associated with the index or identifier
	 *	 Invocables.Hook.Call(Ivocables.HOOKS.USED_ITEM, "msg2");
	 *	 Invocables.Hook.Call("HOOK_TEST", "value1", 2, "value3");
 *
 * Print all parameter values through lambda functions:
 *	 //Lambda function hook to print all the values of each parameter passed in the log
 *		string msg = "Hook Call 1";
 *		Invocables.Hook.Add(1, (para) => {
 *				System.Array.ForEach(para, UnityEngine.Debug.Log);
 *			}, msg, 4, "parameters"
 *		);
 * 
 * Other Implementations:
 * -Garrysmod's LUA API: http://maurits.tv/data/garrysmod/wiki/wiki.garrysmod.com/index82fb.html
 *                       http://maurits.tv/data/garrysmod/wiki/wiki.garrysmod.com/indexd101.html
 * -Most extensibile games and those with bindings for scripted languages.
 * 
 * Future Additions:
 * -Event blocking. Good for modular classes to return a value to block an event from being called.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;

namespace Invocables {
	public static class Hook {
		static List<Item>[] hooks;
		static Dictionary<string, int> hookStrings;

		static Hook() {
			//List<int> yo = new List<int>();
			//yo.Add(4);
			//yo.FindIndex(v => v == this);

			var values = Enum.GetValues(typeof(HOOKS));

			hooks = new List<Item>[values.Length];
			hookStrings = new Dictionary<string, int>();

			foreach(HOOKS val in values)
			{
				UnityEngine.Debug.Log (String.Format("{0}: {1}", Enum.GetName(typeof(HOOKS), val), val));
				Hook.Create(val, Enum.GetName(typeof(HOOKS), val));
			}
		}
		
		public static void Create(HOOKS hook, string identifier) {
			int index = HookToIndex(hook);
			if (System.Diagnostics.Debugger.IsAttached){
				if (index < hooks.Length ? hooks[index] != null : false)
					throw new Exception(String.Format("Error: Hook index '{0}' already exists.", index));
				if (hookStrings.ContainsKey(identifier))
					throw new Exception(String.Format("Error: Hook indentifier '{0}' already exists.", identifier));
			}
			
			if (index >= hooks.Length)
				Array.Resize(ref hooks, index + 1); //As we're using primitive arrays, resize if the id is larger than the allocated size
			
			hooks[index] = new List<Item>(){};
			hookStrings.Add(identifier, index);
		}
		
		public static void Add(HOOKS hook, Action<System.Object[]> funct) {
			AddFunct(GetActionList(HookToIndex(hook)), new Item(funct));
		}
		
		public static void Add(string identifier, Action<System.Object[]> funct) {
			AddFunct(GetActionList(identifier), new Item(funct));
		}
		
		public static void Add(HOOKS hook, Action funct) {
			AddFunct(GetActionList(HookToIndex(hook)), new ItemNoPara(funct));
		}
		
		public static void Add(string identifier, Action funct) {
			AddFunct(GetActionList(identifier), new ItemNoPara(funct));
		}
		
		public static void Call(HOOKS hook) {
			GetActionList(HookToIndex(hook)).ForEach(Invoke);
		}
		
		public static void Call(string identifier) {
			GetActionList(identifier).ForEach(Invoke);
		}

		public static void Call(HOOKS hook, params System.Object[] para) {
			Item.para = para;
			Call(hook);
		}
		
		public static void Call(string identifier, params System.Object[] para) {
			Item.para = para;
			Call(identifier);
		}

		public static void Reserve(int size) {
			if (size < hooks.Length)
				Array.Resize(ref hooks, size);
		}

		private static int HookToIndex(HOOKS hook) {
			return (int) hook;
		}
		
		private static void AddFunct(List<Item> list, Item item) {
			list.Add(item);
		}
		
		private static void Invoke(Item item) {
			item.Invoke();
		}
		
		private static List<Item> GetActionList(int index) {
			if (System.Diagnostics.Debugger.IsAttached){
				if ((index < hooks.Length && hooks[index] == null) || index >= hooks.Length)
					throw new Exception(String.Format("Error: Hook index '{0}' doesn't exist.", index));
			}
			
			return hooks[index];
		}
		
		private static List<Item> GetActionList(string identifier) {
			if (System.Diagnostics.Debugger.IsAttached){
				if (!hookStrings.ContainsKey(identifier))
					throw new Exception(String.Format("Error: Hook identifier '{0}' doesn't exist.", identifier));
			}
			
			int index = hookStrings[identifier];
			return GetActionList(index);
		}
		
		public class Item {
			static public System.Object[] para;

			protected Action functNoPara;
			private Action<System.Object[]> funct;

			public Item(Action<System.Object[]> funct) {
				this.funct = funct;
			}
			
			public virtual void Invoke() {
				funct(para);
			}
		}
		
		public class ItemNoPara : Item {
			public ItemNoPara(Action funct): base(null) {
				this.functNoPara = funct;
			}
			
			public override void Invoke() {
				functNoPara();
			}
		}
	}
}
