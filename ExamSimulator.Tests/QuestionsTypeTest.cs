using ExamSimulator.Domain.Models;
using ExamSimulator.Domain.Enums;
using ExamSimulator.Domain.Structs;

namespace ExamSimulator.Tests
{
    public class QuestionTypeTests
    {
        [Test]
        public void SingleChoiceQuestion_CalculateScore_CorrectAnswer_ReturnsOne()
        {
            SingleChoiceQuestion question = new SingleChoiceQuestion 
            { 
                Text = "Який тип даних для цілих чисел?", 
                Difficulty = DifficultyLevel.Easy 
            };

            question.Options.Add(new AnswerOption { Text = "int", IsCorrect = true });
            question.Options.Add(new AnswerOption { Text = "string", IsCorrect = false });

            List<string> userAnswers = new List<string>();
            userAnswers.Add("int");

            double score = question.CalculateScore(userAnswers);

            Assert.That(score, Is.EqualTo(1.0)); 
        }

        [Test]
        public void SingleChoiceQuestion_CalculateScore_WrongAnswer_ReturnsZero()
        {
            SingleChoiceQuestion question = new SingleChoiceQuestion 
            { 
                Text = "Який тип даних для цілих чисел?", 
                Difficulty = DifficultyLevel.Easy 
            };

            question.Options.Add(new AnswerOption { Text = "int", IsCorrect = true });
            question.Options.Add(new AnswerOption { Text = "string", IsCorrect = false });

            List<string> userAnswers = new List<string>();
            userAnswers.Add("string");

            double score = question.CalculateScore(userAnswers);

            Assert.That(score, Is.EqualTo(0.0)); 
        }

        [Test]
        public void MultipleChoiceQuestion_CalculateScore_AllCorrectSelected_ReturnsOne()
        {
            MultipleChoiceQuestion question = new MultipleChoiceQuestion { Text = "Цикли?", Difficulty = DifficultyLevel.Medium };
            question.Options.Add(new AnswerOption { Text = "for", IsCorrect = true });
            question.Options.Add(new AnswerOption { Text = "while", IsCorrect = true });
            question.Options.Add(new AnswerOption { Text = "if", IsCorrect = false });

            List<string> userAnswers = new List<string>();
            userAnswers.Add("for");
            userAnswers.Add("while");

            double score = question.CalculateScore(userAnswers);

            Assert.That(score, Is.EqualTo(1.0));
        }

        [Test]
        public void MultipleChoiceQuestion_CalculateScore_PartialOrWrong_ReturnsZero()
        {
            MultipleChoiceQuestion question = new MultipleChoiceQuestion { Text = "Цикли?", Difficulty = DifficultyLevel.Medium };
            question.Options.Add(new AnswerOption { Text = "for", IsCorrect = true });
            question.Options.Add(new AnswerOption { Text = "while", IsCorrect = true });
            
            List<string> userAnswers = new List<string>();
            userAnswers.Add("for");

            double score = question.CalculateScore(userAnswers);

            Assert.That(score, Is.LessThan(1.0));
        }

        [Test]
        public void OpenQuestion_CalculateScore_ExactMatch_ReturnsOne()
        {
            OpenQuestion question = new OpenQuestion { Text = "2 + 2 = ?", CorrectAnswerText = "4", Difficulty = DifficultyLevel.Easy };
            
            List<string> userAnswers = new List<string>();
            userAnswers.Add("4");

            double score = question.CalculateScore(userAnswers);

            Assert.That(score, Is.EqualTo(1.0));
        }

        [Test]
        public void OpenQuestion_CalculateScore_WrongMatch_ReturnsZero()
        {
            OpenQuestion question = new OpenQuestion { Text = "2 + 2 = ?", CorrectAnswerText = "4", Difficulty = DifficultyLevel.Easy };
            
            List<string> userAnswers = new List<string>();
            userAnswers.Add("5");

            double score = question.CalculateScore(userAnswers);

            Assert.That(score, Is.EqualTo(0.0));
        }
    }
}