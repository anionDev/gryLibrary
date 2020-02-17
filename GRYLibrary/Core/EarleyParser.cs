using System;
using System.Collections.Generic;
using System.Linq;

namespace GRYLibrary.Core
{
    public class EarleyParser
    {
        private SortedSet<State>[] S;

        private void Initialize(object[] words)
        {
            S = new SortedSet<State>[words.Length + 1];
            for (int k = 0; k <= words.Length; k++)
            {
                S[k] = new SortedSet<State>();
            }
        }
        public SortedSet<State>[] Parse(object[] words, Grammar grammar)
        {
            Initialize(words);
            // S[0].Add(new State() { Rule = new ProductionRuleWithExpection() { Identifier = new NonTerminalSymbol() { Value = string.Empty }, Substituted = new List<Symbol>(), Expected  = S }, J = 0 });
            for (int k = 0; k <= words.Length; k++)
            {
                foreach (State state in S[k]) // S[k] can expand during this loop
                {
                    if (Finished(state))
                    {
                        if (NextElementOfState(state) is TerminalSymbol)
                        {
                            Scanner(state, k, words);
                        }
                        else
                        {
                            Predictor(state, k, grammar);
                        }
                    }
                    else
                    {
                        Completer(state, k);
                    }
                }
            }
            return S;
        }

        private Symbol NextElementOfState(State state)
        {
            throw new NotImplementedException();
        }

        private bool Finished(State state)
        {
            throw new NotImplementedException();
        }

        private void Predictor(State state, int k, Grammar grammar)
        {
            foreach (ProductionRule rule in GrammarRules(state.Rule.Expected, grammar))
            {
                S[k].Add(new State()
                {
                    Rule = new ProductionRuleWithExpection()
                    {
                        Identifier = (NonTerminalSymbol)state.Rule.Expected.First(),
                        Substituted = new List<Symbol>(),
                        Expected = rule.Subsitution,
                    },
                    J = k,
                });
            }
        }

        private IEnumerable<ProductionRule> GrammarRules(List<Symbol> B, Grammar grammar)
        {
            throw new NotImplementedException();
        }

        private void Scanner(State state, int k, object[] words)
        {
            if (PartsOfSpeech(words[k]).Contains(state.Rule.Expected.First()))
            {
                S[k + 1].Add(new State()
                {
                    Rule = new ProductionRuleWithExpection()
                    {
                        Identifier = state.Rule.Identifier,
                        Substituted = state.Rule.Substituted.Concat(new List<Symbol>() { state.Rule.Expected.First() }).ToList(),
                        Expected = state.Rule.Expected.Skip(1).ToList()
                    },
                    J = state.J
                });
            }
        }

        private IEnumerable<Symbol> PartsOfSpeech(object word)
        {
            throw new NotImplementedException();
        }

        private void Completer(State state, int k)
        {
            foreach (State state2 in S[state.J])
            {
                S[k].Add(new State()
                {
                    Rule = new ProductionRuleWithExpection()
                    {
                        Identifier = state2.Rule.Identifier,
                        Substituted = state2.Rule.Substituted.Concat(new List<Symbol>() { state2.Rule.Expected.First() }).ToList(),
                        Expected = state2.Rule.Expected.Skip(1).ToList()
                    },
                    J = state2.J
                });
            }
        }
        public class Grammar
        {
            public List<ProductionRule> Rules;
        }
        public abstract class Symbol
        {
            public string Value;
        }
        public class TerminalSymbol : Symbol
        {

        }
        public class NonTerminalSymbol : Symbol
        {

        }
        public class State
        {
            public ProductionRuleWithExpection Rule;
            public int J;
        }
        public class ProductionRuleWithExpection
        {
            public NonTerminalSymbol Identifier;
            public List<Symbol> Substituted;
            public List<Symbol> Expected;
        }
        public class ProductionRule
        {
            public NonTerminalSymbol Identifier;
            public List<Symbol> Subsitution;
        }

    }
}
