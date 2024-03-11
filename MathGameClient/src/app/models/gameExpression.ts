import { AnswerStatus } from "../enums/AnswerStatus";

export interface GameExpression {
    id: number;
    mathExpression: string;
    result: number;
    gameSessionId: number;
    gameSession: any;
    answerStatus: AnswerStatus;
    answeredFromPlayer: string;
    answer: number;
  }
  